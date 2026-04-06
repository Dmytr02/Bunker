Shader "Custom/URP_Universal_Lit"
{
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        
        [MainTexture] _MainTex("Texture", 2D) = "white" {}

        _Shades("Shades", Float) = 3.0
        _Min("Min", Float) = 0.0
        _Max("Max", Float) = 1.0
        _PosterizeSteps("PosterizeSteps", Range(0, 1)) = 0.5

        // ДОБАВЛЕНО: без этих свойств тайлинг текстуры не будет работать
        _Tiling("Tiling", Vector) = (1, 1, 0, 0)
        _Offset("Offset", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _FORWARD_PLUS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : NORMAL;
            };

            float4 _BaseColor; half _Smoothness; half _Metallic;
            sampler2D _MainTex; float2 _Tiling; float2 _Offset;
            float _Shades, _Min, _Max, _PosterizeSteps;

            Varyings vert (Attributes input) {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                // Тайлинг и оффсет теперь берутся из Properties
                output.uv = input.uv * _Tiling + _Offset;
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target {
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(GetWorldSpaceViewDir(input.positionWS));
                
                BRDFData brdfData;
                InitializeBRDFData(float3(1, 1, 1), _Metallic, 0, _Smoothness, _BaseColor.a, brdfData);

                // 1. Считаем свет
                Light mainLight = GetMainLight();
                half3 finalColor = 0;//GlobalIllumination(brdfData, SampleSH(normal), 1.0, normal, viewDir);
                // finalColor += LightingPhysicallyBased(brdfData, mainLight, normal, viewDir);

                // 2. Дополнительный свет (Point/Spot) через макрос
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                // 2. Дополнительный свет (Point/Spot)
                uint pixelLightCount = GetAdditionalLightsCount();
                LIGHT_LOOP_BEGIN(pixelLightCount)
                    Light addLight = GetAdditionalLight(lightIndex, input.positionWS);
                    finalColor += LightingPhysicallyBased(brdfData, addLight, normal, viewDir);
                LIGHT_LOOP_END

                // Получаем цвет из текстуры
                float4 texColor = tex2D(_MainTex, input.uv);
                float toonStep = step(_PosterizeSteps, finalColor);
                toonStep = Remap(0, 1, _Min, _Max, toonStep);
                // Домножаем накопленный свет на текстуру и на _BaseColor
                float4 color = toonStep * texColor * _BaseColor;
                
                return color;
            }
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };
            struct Varyings {
                float4 positionCS : SV_POSITION;
            };

            Varyings ShadowPassVertex(Attributes input) {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET {
                return 0;
            }
            ENDHLSL
        }
    }
}
