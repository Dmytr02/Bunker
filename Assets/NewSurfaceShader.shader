Shader "Custom/URP_Universal_Lit"
{
    Properties
    {
        [MainTexture] _MainTex("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)

        _Shades("Shades", Float) = 3.0
        _Min("Min", Float) = 0.0
        _Max("Max", Float) = 1.0
        
        _Tiling("Tiling", Vector) = (1, 1, 0, 0)
        _Offset("Offset", Vector) = (0, 0, 0, 0)

        _PosterizeSteps("PosterizeSteps", Float) = 5.0
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Smoothness("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Обязательные ключевые слова для работы ламп и Forward+
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _FORWARD_PLUS // Добавлено для новых версий

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
{
    float4 positionOS   : POSITION;
    float2 uv           : TEXCOORD0; 
    float3 normalOS     : NORMAL;
};

struct Varyings
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0; 
    float3 positionWS   : TEXCOORD1;
    float3 normalWS     : NORMAL;
};

            float4 _BaseColor; half _Smoothness; half _Metallic; sampler2D _MainTex;float2 _Tiling;    float2 _Offset;
            float _Shades, _Min, _Max, _PosterizeSteps;

            void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
            {
                Out = floor(In * Steps) / Steps;
            }
            
            Varyings vert (Attributes input) {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);

                output.uv = input.uv * _Tiling + _Offset; 

                return output;
            }

            half4 frag (Varyings input) : SV_Target {
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(GetWorldSpaceViewDir(input.positionWS));
                
                BRDFData brdfData;
                InitializeBRDFData(float3(1, 1, 1), _Metallic, 0, _Smoothness, _BaseColor.a, brdfData);

                Light mainLight = GetMainLight();
                half3 finalColor = GlobalIllumination(brdfData, SampleSH(normal), 1.0, normal, viewDir);
                //finalColor += LightingPhysicallyBased(brdfData, mainLight, normal, viewDir);
                float lightNormal = 0; dot(normal, mainLight.direction);
                float3 lightColor = mainLight.color;
                
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;

                uint pixelLightCount = GetAdditionalLightsCount();
                LIGHT_LOOP_BEGIN(pixelLightCount)
                    Light addLight = GetAdditionalLight(lightIndex, input.positionWS);
                lightNormal = max(lightNormal, dot(addLight.direction,normal));
                lightColor = lightColor + addLight.color;
                    //finalColor += LightingPhysicallyBased(brdfData, addLight, normal, viewDir);
                LIGHT_LOOP_END

                lightNormal = Remap(-1, 1, 0, 1, lightNormal);
                float remap = Remap(0, 1/_Shades, _Min, _Max, floor(lightNormal / _Shades));
                float4 colored = remap * _BaseColor * tex2D(_MainTex, input.uv);
                float4 color;
                Unity_Posterize_float4( colored, _PosterizeSteps, color);
                
                return colored;
            }   
            ENDHLSL
        }
    }
}
