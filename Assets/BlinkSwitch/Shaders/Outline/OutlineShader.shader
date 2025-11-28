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

            sampler2D _CustomDepthNormalTexture;

            float CalculateOutlineSobel(float2 pixelCoord, float2 resolution)
            {
                float2 offsets[9] = {
                    float2(-1,  1), float2(0,  1), float2(1,  1),
                    float2(-1,  0), float2(0,  0), float2(1,  0),
                    float2(-1, -1), float2(0, -1), float2(1, -1)
                };

                float sobelX[9] = { -1, 0, 1,
                                    -2, 0, 2,
                                    -1, 0, 1 };

                float sobelY[9] = {  1,  2,  1,
                                     0,  0,  0,
                                    -1, -2, -1 };

                float2 uvCenter = pixelCoord / resolution;
                float4 centerDepthNormal = tex2D(_CustomDepthNormalTexture, uvCenter);

                float gradNormalX = 0;
                float gradNormalY = 0;

                float gradDepthX = 0;
                float gradDepthY = 0;

                for (int i = 0; i < 9; i++)
                {
                    float2 uv = (pixelCoord + offsets[i] * _OutlineSize) / resolution;

                    float4 neighbourDepthNormal = tex2D(_CustomDepthNormalTexture, uv);

                    float nd = 1.0f - dot(centerDepthNormal.yzw, neighbourDepthNormal.yzw);
                    float dd = abs(centerDepthNormal.r - neighbourDepthNormal.r);

                    gradNormalX += nd * sobelX[i];
                    gradNormalY += nd * sobelY[i];

                    gradDepthX += dd * sobelX[i];
                    gradDepthY += dd * sobelY[i];
                }

                float normalMagnitude = length(float2(gradNormalX, gradNormalY));
                float depthMagnitude  = length(float2(gradDepthX, gradDepthY));

                float outline = 1.0f - (step(_OutlineNormalThreshold, normalMagnitude) +
                                step(_OutlineDepthThreshold, depthMagnitude));

                return saturate(outline);
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float outline = CalculateOutlineSobel(i.uv * _ScreenParams.xy, _ScreenParams.xy);
                return float4(outline, outline, outline, 1.0f);
            }
            ENDCG
        }
    }
}
