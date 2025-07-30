Shader "BlinkSwitch/OldTvPostProcessShader"
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

            float3 jodieReinhardTonemap(float3 c){
                float l = dot(c, float3(0.2126, 0.7152, 0.0722));
                float3 tc = c / (c + 1.0);

                return lerp(c / (l + 1.0), tc, tc);
            }

            float _Curvature;
            float _MinLuminanceThreshold;
            float _MaxLuminanceThreshold;
            float _OldTvPixelSize;

            float4 frag (v2f i) : SV_Target
            {
                float2 fragCoord = i.uv * _ScreenParams.xy;
                float2 uv = floor(fragCoord / _OldTvPixelSize) * (_OldTvPixelSize / _ScreenParams.xy);
    
                uv -= 0.5f;
    
                float2 offset = uv / _Curvature;
    
                uv = uv + uv * length(offset) * length(offset);
    
                float v = 5.0f;
    
                float2 vignetteVec = float2(v, v);
                uv += 0.5f;
    
                float2 invUv =  1.0f - abs(uv * 2.0f - 1.0f);
    
                float2 vignnete = lerp(float2(0.0f, 0.0f), vignetteVec, invUv);
    
                float x = clamp(step(0.0f, uv.x) * (1.0f - step(1.0f, uv.x)) * vignnete.x, 0.0f, 1.0f);
                float y = clamp(step(0.0f, uv.y) * (1.0f - step(1.0f, uv.y)) * vignnete.y, 0.0f, 1.0f);
    
                float s = ((sin(i.uv.y * _ScreenParams.y) + 1.0f) * 0.1f + 1.0f);
                float c = ((cos(i.uv.y * _ScreenParams.y) + 1.0f) * 0.10f + 1.0f);
    
                
                float3 col = tex2D(_MainTex, uv).rgb;
                float luminance = 0.3f * col.r + 0.59f * col.g + 0.11f * col.b;
                col = col * x * y;
                col.rb *= c;
                col.g *= s;
                float factor = 1.0f / 2.2f;
                col = pow(col, float3(factor, factor, factor));
                col = jodieReinhardTonemap(col);

                return float4(col.rgb * (smoothstep(_MinLuminanceThreshold, _MaxLuminanceThreshold, luminance)), 1.0f);
            }
            ENDCG
        }
    }
}
