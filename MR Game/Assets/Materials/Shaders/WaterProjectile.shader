Shader "Meta/Depth/BiRP/WaterProjectile"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _fresnelIntensity ("Fresnel Intensity", Float) = 0
    }
   SubShader
    {
        Tags{ "RenderType"="Transparent" "Queue"="Transparent"}
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
                float3 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float2 uv0 : TEXCOORD0;
                float3 worldNormal : NORMAL;
                // 3. This macro adds required data field to the varyings struct
                //    The number has to be filled with the recent TEXCOORD number + 1
                //    Or 0 as in this case, if there are no other TEXCOORD fields
                META_DEPTH_VERTEX_OUTPUT(0)

                UNITY_VERTEX_INPUT_INSTANCE_ID
                // 4. The fragment shader needs to understand to which eye it's currently
                //    rendering, in order to get depth from the correct texture.
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _fresnelIntensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionCS = UnityObjectToClipPos(input.vertex);
                output.positionWS = mul(unity_ObjectToWorld, input.vertex).xyz;
                output.uv0 = input.uv0;
                output.worldNormal = UnityObjectToWorldNormal(input.normal);
                // 5. World position is required to calculate the occlusions.
                //    This macro will calculate and set world position value in the output Varyings structure.
                META_DEPTH_INITIALIZE_VERTEX_OUTPUT(output, input.vertex);

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                // 6. Passes stereo information to frag shader
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                return output;
            }
            float3 getCameraDirection(float3 worldPosition) {
                return normalize(_WorldSpaceCameraPos-worldPosition);
            }
            half fresnel(float3 worldNormal, float3 camDirection) {
                float fresnelVal = dot(worldNormal,camDirection);
                return saturate(1 - fresnelVal);
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                // 7. Initializes global stereo constant for the frag shader
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                half3 camdirection = getCameraDirection(input.positionWS);
                half4 colorFromUV = half4(input.uv0,0,1); //creates a colour from the UV0 coordinates
                half4 finalColor = _BaseColor;
                half fresnelNum = fresnel(input.worldNormal,camdirection) + _fresnelIntensity;
                half4 fresnelAlpha = half4(1,1,1,fresnelNum);


                // 8. A third macro required to enable occlusions.
                //    It requires previous macros to be there as well as the naming behind the macro is strict.
                //    It will enable soft or hard occlusions depending on the current keyword set.
                //    finalColor value will be multiplied by the occlusion visibility value.
                //    Occlusion visibility value is 0 if virtual object is completely covered by environment and vice versa.
                //    Fully occluded pixels will be discarded
                META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY(input, finalColor, fresnelNum);

                return _BaseColor*fresnelAlpha;
            }
            ENDCG
        }
    }
}
