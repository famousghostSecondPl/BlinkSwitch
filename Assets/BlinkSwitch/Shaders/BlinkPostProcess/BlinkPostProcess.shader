Shader "BlinkSwitch/BlinkPostProcess"
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

            sampler2D _PostProcessTexture;
            float _Blink;
            float _CurveStrength;
            float _BlurEdgeStrength;

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_PostProcessTexture, i.uv);
                float2 uv = i.uv;
                uv -= 0.5f;
                uv = uv - uv * length(uv) * length(uv) * _CurveStrength;
                uv += 0.5f;
                const float blink = min(_Blink, 0.6f);
                const float oneMinusBlink = 1.0f - blink;
                col = lerp(col, float4(0.0f, 0.0f, 0.0f, 1.0f), 
                           smoothstep(oneMinusBlink, oneMinusBlink + _BlurEdgeStrength, uv.y) + smoothstep(oneMinusBlink, oneMinusBlink + _BlurEdgeStrength, 1.0f - uv.y));
                return col;
            }
            ENDCG
        }
    }
}
