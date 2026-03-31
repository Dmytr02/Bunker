Shader "Custom/URP_Universal_Lit"
{
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        
         // Текстура и основной цвет
        [MainTexture] _MainTex("Texture", 2D) = "white" {}

        // Параметры для стилизации (Toon/Posterize)
        _Shades("Shades", Float) = 3.0
        _Min("Min", Float) = 0.0
        _Max("Max", Float) = 1.0

        // Дополнительные параметры
        _PosterizeSteps("PosterizeSteps", Float) = 5.0
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

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 positionWS : TEXCOORD0; float3 normalWS : TEXCOORD1; };

            float4 _BaseColor; half _Smoothness; half _Metallic;
            sampler2D _MainTex;float2 _Tiling;    float2 _Offset;
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
                return output;
            }

            half4 frag (Varyings input) : SV_Target {
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(GetWorldSpaceViewDir(input.positionWS));
                
                BRDFData brdfData;
                InitializeBRDFData(_BaseColor.rgb, _Metallic, 0, _Smoothness, _BaseColor.a, brdfData);

                // 1. Главный свет
                Light mainLight = GetMainLight();
                half3 finalColor = GlobalIllumination(brdfData, SampleSH(normal), 1.0, normal, viewDir);
                finalColor += LightingPhysicallyBased(brdfData, mainLight, normal, viewDir);

                // 2. Дополнительный свет (Point/Spot) через макрос
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;

                uint pixelLightCount = GetAdditionalLightsCount();
                LIGHT_LOOP_BEGIN(pixelLightCount)
                    Light addLight = GetAdditionalLight(lightIndex, input.positionWS);
                    // Здесь addLight.direction — это направление на Point Light
                    finalColor += LightingPhysicallyBased(brdfData, addLight, normal, viewDir);
                LIGHT_LOOP_END

                float4 color = float4(finalColor, 1.0);
                Unity_Posterize_float4(color, _PosterizeSteps, color);
                
                return color;
            }
            ENDHLSL
        }
    }
}