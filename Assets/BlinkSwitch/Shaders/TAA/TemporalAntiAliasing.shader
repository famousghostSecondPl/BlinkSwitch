
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

            sampler2D _VelocityTexture;
            sampler2D _PreviousVelocityTexture;

            float2 _CurrentFrameJitter;
            float2 _PreviousFrameJitter;

            float3 RGBtoYCoCg(float3 c)
            {
                float Co = c.r - c.b;
                float t  = c.b + Co * 0.5;
                float Cg = c.g - t;
                float Y  = t + Cg * 0.5;
                return float3(Y, Co, Cg);
            }

            float3 YCoCgtoRGB(float3 c)
            {
                float Y = c.x;
                float Co = c.y;
                float Cg = c.z;

                float t = Y - Cg * 0.5;
                float g = Cg + t;
                float b = t - Co * 0.5;
                float r = b + Co;

                return float3(r, g, b);
            }

            int _TaaVersion;

            float4 frag (v2f i) : SV_Target
            { 
                float4 velocity = tex2D(_VelocityTexture, i.uv);


                float3 minColor = 9999.0f;
                float3 maxColor = -9999.0f;
 

                //With shimmering, but eliminated ghosting
                if(_TaaVersion == 1)
                {
                    float2 previousScreenUv = i.uv - velocity.rg;
                    float4 previousVelocity = tex2D(_PreviousVelocityTexture, previousScreenUv);
                    float3 currentColorBlured = float3(0.0f, 0.0f, 0.0f);
                    // Sample a 3x3 neighborhood to create a box in color space
                    for(int x = -1; x <= 1; ++x)
                    {
                        for(int y = -1; y <= 1; ++y)
                        {
                            float3 color = RGBtoYCoCg(tex2D(_MainTex, i.uv + (float2(x, y) / _ScreenParams.xy))); 
                            minColor.yz = min(minColor.yz, color.yz);
                            maxColor.yz = max(maxColor.yz, color.yz);
                            currentColorBlured += color;
                        }
                    }

                    currentColorBlured /= 9.0f;

                    float3 currentColor = RGBtoYCoCg(tex2D(_MainTex, i.uv).rgb);
                    float3 previouColor = RGBtoYCoCg(tex2D(_PreviousFrameTexture, previousScreenUv).rgb);
                    float3 previousColorClamped = clamp(previouColor, minColor, maxColor);

                    float3 col = currentColor * 0.1f + previousColorClamped * 0.9f;

                    float velocityLength = length(previousVelocity.rg - velocity.rg);

                    float reject = saturate((velocityLength - 0.01f) * 10.0f);

                    float3 result = lerp(currentColorBlured, col, 1.0f - reject);

                    return float4(YCoCgtoRGB(result), 1.0f);
                }
                //Without shimmering, but with ghosting
                // Sample a 3x3 neighborhood to create a box in color space

                if(_TaaVersion == 0)
                {
                    float2 previousScreenUv = i.uv + velocity.ba;
                    float4 previousVelocity = tex2D(_PreviousVelocityTexture, previousScreenUv);
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

                    float velocityLength = length(previousVelocity.ba - velocity.ba);

                    float reject = saturate((velocityLength - 0.01f) * 10.0f);

                    float3 col = currentColor * 0.1f + previousColorClamped * 0.9f;

                    float3 result = lerp(currentColor, col, 1.0f - reject);

                    return float4(col, 1.0f);
                }
                return float4(0.0f, 0.0f, 0.0f, 1.0f);
            }
            ENDCG
        }
    }
}
