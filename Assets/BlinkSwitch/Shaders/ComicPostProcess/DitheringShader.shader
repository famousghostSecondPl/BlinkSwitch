Shader "BlinkSwitch/DitheringShader"
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

            float _PixelSize;
            float _BitsPerColor;
            float _DitheringSpreadSize;

            float3 nearestColorPallete(float3 color)
            {
                return floor(color * _BitsPerColor) / _BitsPerColor;
            }

            float3 ditheringColor(float2 fragCoord, float3 color)
            {
                float2x2 thresholdMap;
                thresholdMap[0][0] = -0.5f;
                thresholdMap[0][1] = 0.0f;
                thresholdMap[1][0] = 0.25f;
                thresholdMap[1][1] = -0.25f;
    
                float3 result = nearestColorPallete(color + _DitheringSpreadSize * (thresholdMap[int(fragCoord.x % 2.0f)][int(fragCoord.y % 2.0f)]));
    
                return result;
    
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            sampler2D _OutlineTexture;

            float4 frag (v2f i) : SV_Target
            {
                float2 fragCoord = _ScreenParams.xy * i.uv;
                float2 pixelSize = float2(_PixelSize, _PixelSize);
                float2 pixelateUvs = floor(fragCoord / pixelSize) * (pixelSize / _ScreenParams.xy);
                float4 col = tex2D(_MainTex, pixelateUvs);
                float outline = tex2D(_OutlineTexture, i.uv).r;
                col.rgb = ditheringColor(fragCoord, col.rgb) * outline;

                return col;
            }
            ENDCG
        }
    }
}
