Shader "Meta/Depth/BiRP/HealthBarShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _FillAmount("Fill Amount", Float) = 1
        _MainTex("Texture", 2D) = "white" {}
        _AlphaClipThreshold("Alpha Clipping Threshold", Range(0, 1)) = 0.5
    }

        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        // 0. It's important to have One OneMinusSrcAlpha so it blends properly against transparent background (passthrough)
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            // 1. Keywords are used to enable different occlusions
            #pragma multi_compile _ HARD_OCCLUSION SOFT_OCCLUSION

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // 2. Include the file with utility functions
            #include "Packages/com.meta.xr.depthapi/Runtime/BiRP/EnvironmentOcclusionBiRP.cginc"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                // 3. This macro adds required data field to the varyings struct
                //    The number has to be filled with the recent TEXCOORD number + 1
                //    Or 0 as in this case, if there are no other TEXCOORD fields
                META_DEPTH_VERTEX_OUTPUT(2)

                UNITY_VERTEX_INPUT_INSTANCE_ID
                    // 4. The fragment shader needs to understand to which eye it's currently
                    //    rendering, in order to get depth from the correct texture.
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                CBUFFER_START(UnityPerMaterial)
                    half4 _BaseColor;
                    half _FillAmmount;
                    sampler2D _MainTex;
                    float _AlphaClipThreshold;
                CBUFFER_END

                Varyings vert(Attributes input)
                {
                    Varyings output;

                    output.positionCS = UnityObjectToClipPos(input.vertex);
                    output.uv0 = input.uv0;


                    META_DEPTH_INITIALIZE_VERTEX_OUTPUT(output, input.vertex);

                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_TRANSFER_INSTANCE_ID(input, output);
                    // 6. Passes stereo information to frag shader
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                    return output;
                }

                half4 oneMinus(half4 input) {
                    return half4(1 - input.x, 1 - input.y, 1 - input.z, 1 - input.w);
                }
                half oneMinus(half input) {
                    return 1 - input;
                }

                half4 frag(Varyings input) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(input);
                // 7. Initializes global stereo constant for the frag shader
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                half4 colorFromUV = half4(input.uv0,0,1); //creates a colour from the UV0 coordinates
                half redComponent = input.uv0.x;
                half4 finalColor = half4(oneMinus(redComponent), 0, 0, 1);



                finalColor.a *= step(_AlphaClipThreshold, finalColor.a); //Alpha clipping
                META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY(input, finalColor, 1);

                return finalColor;
            }
            ENDCG
        }
    }
}