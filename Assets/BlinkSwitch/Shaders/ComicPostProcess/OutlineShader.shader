Shader "BlinkSwitch/OutlineShader"
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
            sampler2D _CameraDepthNormalsTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float _OutlineSize;
            float _OutlineDepthThreshold;
            float _OutlineNormalThreshold;

            float CalculateOutline(float2 uv, float2 resolution)
            {
                float4 depthNormal = tex2D(_CameraDepthNormalsTexture, uv);
                float currentDepth;
                float3 currentNormal;
                DecodeDepthNormal(depthNormal, currentDepth, currentNormal);
                float currentLinearDepth = LinearEyeDepth(currentDepth);
                for(int x = -1; x <= 1; ++x)
                {
                    for(int y = -1; y <= 1; ++y)
                    {
                        if(x == 0 && y == 0)
                        {
                            continue;
                        }
                        float4 neighbourDepthNormal = tex2D(_CameraDepthNormalsTexture, uv + (float2(float(x), float(y)) * _OutlineSize) / resolution);
                        float neighbourDepth;
                        float3 neighbourNormal;
                        DecodeDepthNormal(neighbourDepthNormal, neighbourDepth, neighbourNormal);
                        float neighbourLinearDepth = LinearEyeDepth(neighbourDepth);
                        if((currentLinearDepth - neighbourLinearDepth) > _OutlineDepthThreshold
                           || dot(currentNormal, neighbourNormal) <= _OutlineNormalThreshold)
                        {
                            return 0.0f;
                        }
                    }
                }
                return 1.0f;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float outline = CalculateOutline(i.uv, _ScreenParams.xy);
                return float4(outline, outline, outline, 1.0f);
            }
            ENDCG
        }
    }
}
