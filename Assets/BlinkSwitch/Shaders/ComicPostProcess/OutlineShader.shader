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

            float4 calculateNormalMapSobel(sampler2D tex, float2 pixelCoord, float2 resolution)
            {
                float2 dir = float2(1.0f, 1.0f) * _OutlineSize;
                
                float2 centerUv = (pixelCoord / resolution);
                float2 uvLeft =    (pixelCoord + float2(-dir.x, 0.0f)) / resolution;
                float2 uvLeftTop = (pixelCoord + float2(-dir.x, dir.y)) / resolution;
                float2 uvTop =     (pixelCoord + float2(0.0f, dir.y)) / resolution;
                float2 uvRightTop =(pixelCoord + float2(dir.x, dir.y)) / resolution;
                float2 uvRight =   (pixelCoord + float2(dir.x, 0.0f)) / resolution;
                float2 uvRightBot =(pixelCoord + float2(dir.x,-dir.y)) / resolution;
                float2 uvBot =     (pixelCoord + float2(0.0f, -dir.y)) / resolution;
                float2 uvLeftBot = (pixelCoord + float2(-dir.x, -dir.y)) / resolution;

                float4 leftTopDepthNormal = tex2D(_CameraDepthNormalsTexture, uvLeftTop);
                float leftTopDepth;
                float3 leftTopNormal;
                DecodeDepthNormal(leftTopDepthNormal, leftTopDepth, leftTopNormal);

                float4 leftDepthNormal = tex2D(_CameraDepthNormalsTexture, uvLeft);
                float leftDepth;
                float3 leftNormal;
                DecodeDepthNormal(leftDepthNormal, leftDepth, leftNormal);

                float4 leftBottomDepthNormal = tex2D(_CameraDepthNormalsTexture, uvLeftBot);
                float leftBottomDepth;
                float3 leftBottomNormal;
                DecodeDepthNormal(leftBottomDepthNormal, leftBottomDepth, leftBottomNormal);

                float4 rightTopDepthNormal = tex2D(_CameraDepthNormalsTexture, uvRightTop);
                float rightTopDepth;
                float3 rightTopNormal;
                DecodeDepthNormal(rightTopDepthNormal, rightTopDepth, rightTopNormal);
                
                float4 rightDepthNormal = tex2D(_CameraDepthNormalsTexture, uvRight);
                float rightDepth;
                float3 rightNormal;
                DecodeDepthNormal(rightDepthNormal, rightDepth, rightNormal);

                float4 rightBottomDepthNormal = tex2D(_CameraDepthNormalsTexture, uvRightBot);
                float rightBottomDepth;
                float3 rightBottomNormal;
                DecodeDepthNormal(rightBottomDepthNormal, rightBottomDepth, rightBottomNormal);

                float4 topDepthNormal = tex2D(_CameraDepthNormalsTexture, uvTop);
                float topDepth;
                float3 topNormal;
                DecodeDepthNormal(topDepthNormal, topDepth, topNormal);

                float4 bottomDepthNormal = tex2D(_CameraDepthNormalsTexture, uvBot);
                float bottomDepth;
                float3 bottomNormal;
                DecodeDepthNormal(bottomDepthNormal, bottomDepth, bottomNormal);
    
                float4 centerDepthNormal = tex2D(_CameraDepthNormalsTexture, centerUv);
                float centerDepth;
                float3 centerNormal;
                DecodeDepthNormal(centerDepthNormal, centerDepth, centerNormal);
                float2 normalSobel = float2(dot(centerNormal, leftTopNormal * -1.0f + leftNormal * -2.0f + leftBottomNormal * -1.0f + rightTopNormal + rightNormal * 2.0f + rightBottomNormal), 
                            dot(centerNormal, leftTopNormal * -1.0f + topNormal * -2.0f + rightTopNormal * -1.0f + leftBottomNormal + bottomNormal * 2.0f + rightBottomNormal));
                float2 depthSobel = float2((leftTopDepth * -1.0f + leftDepth * -2.0f + leftDepth * -1.0f + rightTopDepth + rightDepth * 2.0f + rightBottomDepth), 
                                           (leftTopDepth * -1.0f + topDepth * -2.0f + rightDepth * -1.0f + leftBottomDepth + bottomDepth * 2.0f + rightBottomDepth));
                return float4(normalSobel, depthSobel);
            }

            float CalculateOutlineSobel(sampler2D tex, float2 pixelCoord, float2 resolution)
            {
                float4 sobelResult = calculateNormalMapSobel(tex, pixelCoord, resolution);
                float2 normalSobel = sobelResult.xy;
                float2 depthSobel = sobelResult.zw;
                float4 centerDepthNormal = tex2D(_CameraDepthNormalsTexture, pixelCoord / resolution);
                float centerDepth;
                float3 centerNormal;
                DecodeDepthNormal(centerDepthNormal, centerDepth, centerNormal);
                return (1.0f - step(_OutlineNormalThreshold, length(normalSobel))) * (1.0f - step(_OutlineDepthThreshold, length(depthSobel)));
            }

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
                //float outline = CalculateOutline(i.uv, _ScreenParams.xy);
                float outline = CalculateOutlineSobel(_CameraDepthNormalsTexture, i.uv * _ScreenParams.xy, _ScreenParams.xy);
                return float4(outline, outline, outline, 1.0f);
            }
            ENDCG
        }
    }
}
