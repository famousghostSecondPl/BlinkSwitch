
Shader "BlinkSwitch/TemporalAntiAliasing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            sampler2D _PreviousFrameTexture;
            float4x4 _PreviousViewProjectionMatrix;

            sampler2D _WorldPosFromDepthTexture;
            sampler2D _MotionVectorTexture;

            float2 _CurrentFrameJitter;
            float2 _PreviousFrameJitter;

            float4 frag (v2f i) : SV_Target
            { 
                float4 currentWorldPos = tex2D(_WorldPosFromDepthTexture, i.uv);
                float4 velocity = tex2D(_MotionVectorTexture, i.uv);
                float4 clipPos = mul(_PreviousViewProjectionMatrix, float4(currentWorldPos.xyz, 1.0f));
                float4 ndc = clipPos / clipPos.w;
                float2 previousScreenUv = ndc.xy * 0.5f + 0.5f;
                previousScreenUv -= _PreviousFrameJitter;
                previousScreenUv += _CurrentFrameJitter;

                float3 minColor = 9999.0f;
                float3 maxColor = -9999.0f;
 
                // Sample a 3x3 neighborhood to create a box in color space
                for(int x = -1; x <= 1; ++x)
                {
                    for(int y = -1; y <= 1; ++y)
                    {
                        float3 color = tex2D(_MainTex, i.uv + (float2(x, y) / _ScreenParams.xy)); 
                        minColor = min(minColor, color);
                        maxColor = max(maxColor, color);
                    }
                }

                float3 currentColor = tex2D(_MainTex, i.uv).rgb;
                float3 previouColor = tex2D(_PreviousFrameTexture, previousScreenUv).rgb;
                float3 previousColorClamped = clamp(previouColor, minColor, maxColor);

                float3 col = lerp(currentColor, previousColorClamped, 0.85f);

                float velocityLength = length(velocity.rg);

                float reject = saturate(velocityLength * 50.0f);

                float3 result = lerp(currentColor, col, 1.0f - reject);

                return float4(col, 1.0f);
            }
            ENDCG
        }
    }
}
