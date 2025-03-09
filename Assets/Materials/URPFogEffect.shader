Shader "Custom/URPFogEffect"
{

    Properties
    {
        _FogStart("Fog Start Distance", Float) = 0.0
        _FogColor("Fog Color", Color) = (1, 1, 1, 1)
        _FogDensity("Fog Density", Float) = 0.5
        _FogBlendRange("Fog Blend Range", Float) = 50.0
        _ShouldFogSkybox("Should Fog Skybox", Float) = 0.0 // 0.0 as false, 1.0 as true
        _FogMinHeight("Fog Min Height", Float) = 0
        _FogBlendRangeHeight("Fog Blend Range Height", Float) = 0
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

            float4 _FogColor;
            float _FogDensity;
            float _FogStart;
            float _FogBlendRange;
            float _ShouldFogSkybox;
            float _FogMinHeight;
            float _FogBlendRangeHeight;

            struct Attributes
            {
                float4 vertex : POSITION;
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MyCameraDepthTexture);

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0; // Add this line to pass world position
                float4 screenPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                OUT.pos = TransformObjectToHClip(IN.vertex.xyz);
                OUT.screenPos = ComputeScreenPos(OUT.pos);
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);
                 OUT.pos = TransformObjectToHClip(IN.vertex.xyz);
                return OUT;
            }


            half4 frag(Varyings IN) : SV_Target
            {
                float2 UV = IN.pos.xy / _ScaledScreenParams.xy;
                real realDepth = SampleSceneDepth(UV);
                float3 worldPos = ComputeWorldSpacePosition(UV, realDepth, UNITY_MATRIX_I_VP);
                float2 uv = IN.screenPos.xy / IN.screenPos.w;
                float rawDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                float depth = Linear01Depth(rawDepth, _ZBufferParams);

                // Sample the original scene color
                half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                float fogFactor;

                // Height-based fog factor
                float height = IN.worldPos.y; // Height in world space

                // Calculate the fog factor

                if (depth <= _FogStart)
                {
                    fogFactor = 0.0; // No fog
                }
                else if (_ShouldFogSkybox > 0.5 && depth >= 1)
                {
                    if (worldPos.y < _FogMinHeight)
                    {
                        float heightDifference = _FogMinHeight - worldPos.y;
                        // Calculate a gradient value between 0 and 1 based on worldPos.y
                        // The gradient will be 0 at _FogMinHeight and transition towards 1 as worldPos.y decreases
                        float gradient = 1.0 - exp(
                            -heightDifference * heightDifference / (_FogBlendRangeHeight * _FogBlendRangeHeight));

                        // We subtract from 1 because we want the fog to be denser at lower heights
                        fogFactor = gradient;
                    }
                    else
                    {
                        fogFactor = 0.0; // No fog
                    }
                }
                else if (depth > _FogStart)
                {
                    // Calculate an exponential fog factor for a more natural fog effect
                    float distanceIntoFog = depth - _FogStart;
                    fogFactor = 1.0 - exp(-distanceIntoFog * distanceIntoFog / (_FogBlendRange * _FogBlendRange));
                }
                else
                {
                    fogFactor = 1.0; // Full fog
                }

                fogFactor = saturate(fogFactor); // Ensure the value is between 0 and 1

                //return float4(IN.worldPos.x,IN.worldPos.y,IN.worldPos.z,1);
                return lerp(sceneColor, _FogColor, fogFactor);
            }
            ENDHLSL
        }
    }
}