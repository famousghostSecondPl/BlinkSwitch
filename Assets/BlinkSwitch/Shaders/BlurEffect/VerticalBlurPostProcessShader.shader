Shader "Unlit/VerticalBlurPostProcessShader"
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

            float _BlurStrength;
            int _BlurSteps;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 VerticalBlur(float2 uv, float2 resolution)
            {
                float3 col = float3(0.0f, 0.0f, 0.0f);
                int blurStep = _BlurSteps / 2;
                float steps = 0.0f;
                for(int y = -blurStep; y <= blurStep; ++y)
                {
                    col += tex2D(_MainTex, uv + float2(0.0f, float(y) * _BlurStrength) / resolution);
                    steps += 1.0f;
                }
                return col / steps;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = float4(VerticalBlur(i.uv, _ScreenParams.xy), 1.0f);
                return col;
            }
            ENDCG
        }
    }
}
