Shader "Meta/Depth/BiRP/FireEnemy"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainColor] _YellowColor("Yellow Color", Color) = (1, 1, 1, 1)
        _ScrollAmount("Scroll Amount", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma multi_compile _ HARD_OCCLUSION SOFT_OCCLUSION

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
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
                META_DEPTH_VERTEX_OUTPUT(2)
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _YellowColor;
                float _ScrollAmount;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = UnityObjectToClipPos(input.vertex);
                output.positionWS = mul(unity_ObjectToWorld, input.vertex).xyz;
                output.uv0 = input.uv0;
                output.worldNormal = UnityObjectToWorldNormal(input.normal);
                META_DEPTH_INITIALIZE_VERTEX_OUTPUT(output, input.vertex);
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                return output;
            }

            // Simple noise functions provided by Unity
            inline float unity_noise_randomValue(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            inline float unity_noise_interpolate(float a, float b, float t)
            {
                return (1.0 - t) * a + (t * b);
            }

            inline float unity_valueNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = unity_noise_randomValue(c0);
                float r1 = unity_noise_randomValue(c1);
                float r2 = unity_noise_randomValue(c2);
                float r3 = unity_noise_randomValue(c3);

                float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
                float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
                float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
                return t;
            }

            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
            {
                float t = 0.0;

                for(int i = 0; i < 3; i++)
                {
                    float freq = pow(2.0, float(i));
                    float amp = pow(0.5, float(3 - i));
                    t += unity_valueNoise(UV * Scale / freq) * amp;
                }

                Out = t;
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
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        // Declare and calculate noiseValue
        float noiseValue;
        

        half3 camdirection = getCameraDirection(input.positionWS);
        half4 colorFromUV = half4(input.uv0,0,1); //creates a colour from the UV0 coordinates
        float2 scrolledUV = input.uv0 + float2(_SinTime.x, _Time.x);
        // Define ember colors (oranges and reds) and bright color
        half4 emberColor = _BaseColor; // Orange-red color
        half4 brightColor = half4(1.0, 1.0, 0.8, 1.0); // Bright yellow-white color
        half fresnelNum = fresnel(input.worldNormal,camdirection) + 0.4;
        fresnelNum = saturate(fresnelNum);

        Unity_SimpleNoise_float(scrolledUV, 100.0, noiseValue); // Adjust scale as needed
        // Initialize finalColor with emberColor
         half4 finalColor = emberColor;

        if (noiseValue > 0.75) {
            finalColor = brightColor;
        }
        else if(noiseValue > 0.4)
        {
        // Remap noiseValue from 0.4-0.75 to 0-1
        float remappedNoiseValue = (noiseValue - 0.4) / (0.75 - 0.4);
        remappedNoiseValue = saturate(remappedNoiseValue); // Clamp value between 0 and 1

        // Blend ember color with the remapped noise value
        finalColor = lerp(_BaseColor, _YellowColor, remappedNoiseValue);    
        }
        else {
        finalColor = _BaseColor;
        }

        finalColor = saturate(finalColor);
        finalColor *= fresnelNum;

        META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY(input, finalColor, fresnelNum);

        return finalColor;
    }


            ENDCG
        }
    }
}
