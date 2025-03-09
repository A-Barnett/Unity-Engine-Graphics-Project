Shader "CustomRenderTexture/CustomLensFlare"
{
    Properties
    {
        _MinBrightness("Minimum Brightness", Float) = 0.7
        _SpilloverIntensity("Spillover Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _MinBrightness;
            float _SpilloverIntensity;


            struct Varyings
            {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                OUT.pos = TransformObjectToHClip(IN.vertex.xyz);
                OUT.screenPos = ComputeScreenPos(OUT.pos);
                return OUT;
            }


            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.screenPos.xy / IN.screenPos.w;

                // Sample the original scene color
                float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                float3 spilloverColor = float3(0, 0, 0);

                // Implement spillover effect
                int radius = 2; // You can adjust the radius if needed
                for (int i = -radius; i <= radius; i++)
                {
                    for (int j = -radius; j <= radius; j++)
                    {
                        float2 offset = float2(i, j) / _ScreenParams.xy * _SpilloverIntensity;
                        float4 sampleColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset);

                        // Accumulate color based on brightness exceeding _MinBrightness
                        if (max(max(sampleColor.r, sampleColor.g), sampleColor.b) > _MinBrightness)
                        {
                            spilloverColor += sampleColor.rgb;
                        }
                    }
                }

                // Calculate brightness and flare effect
                float brightness = max(max(sceneColor.r, sceneColor.g), sceneColor.b);
                float3 flareColor = float3(1.0, 0.5, 0.2); // Orange flare color
                float flareIntensity = saturate((brightness - _MinBrightness) * 5.0);

                // Additive blend for the spillover effect
                float3 finalColor = sceneColor.rgb + spilloverColor * flareIntensity;

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}