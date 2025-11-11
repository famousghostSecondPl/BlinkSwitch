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
                float leftTopDepth01 = leftTopDepth;

                float4 leftDepthNormal = tex2D(_CameraDepthNormalsTexture, uvLeft);
                float leftDepth;
                float3 leftNormal;
                DecodeDepthNormal(leftDepthNormal, leftDepth, leftNormal);
                float leftDepth01 = leftDepth;

                float4 leftBottomDepthNormal = tex2D(_CameraDepthNormalsTexture, uvLeftBot);
                float leftBottomDepth;
                float3 leftBottomNormal;
                DecodeDepthNormal(leftBottomDepthNormal, leftBottomDepth, leftBottomNormal);
                float leftBottomDepth01 = leftBottomDepth;

                float4 rightTopDepthNormal = tex2D(_CameraDepthNormalsTexture, uvRightTop);
                float rightTopDepth;
                float3 rightTopNormal;
                DecodeDepthNormal(rightTopDepthNormal, rightTopDepth, rightTopNormal);
                float rightTopDepth01 = rightTopDepth;
                
                float4 rightDepthNormal = tex2D(_CameraDepthNormalsTexture, uvRight);
                float rightDepth;
                float3 rightNormal;
                DecodeDepthNormal(rightDepthNormal, rightDepth, rightNormal);
                float rightDepth01 = rightDepth;

                float4 rightBottomDepthNormal = tex2D(_CameraDepthNormalsTexture, uvRightBot);
                float rightBottomDepth;
                float3 rightBottomNormal;
                DecodeDepthNormal(rightBottomDepthNormal, rightBottomDepth, rightBottomNormal);
                float rightBottomDepth01 = rightBottomDepth;

                float4 topDepthNormal = tex2D(_CameraDepthNormalsTexture, uvTop);
                float topDepth;
                float3 topNormal;
                DecodeDepthNormal(topDepthNormal, topDepth, topNormal);
                float topDepth01 = topDepth;

                float4 bottomDepthNormal = tex2D(_CameraDepthNormalsTexture, uvBot);
                float bottomDepth;
                float3 bottomNormal;
                DecodeDepthNormal(bottomDepthNormal, bottomDepth, bottomNormal);
                float bottomDepth01 = bottomDepth;
    
                float4 centerDepthNormal = tex2D(_CameraDepthNormalsTexture, centerUv);
                float centerDepth;
                float3 centerNormal;
                DecodeDepthNormal(centerDepthNormal, centerDepth, centerNormal);
                float centerDepth01 = centerDepth;
                float2 normalSobel = float2(length(leftTopNormal * -1.0f + leftNormal * -2.0f + leftBottomNormal * -1.0f + rightTopNormal + rightNormal * 2.0f + rightBottomNormal), 
                            length(leftTopNormal + topNormal * 2.0f + rightTopNormal + leftBottomNormal * -1.0f + bottomNormal * -2.0f + rightBottomNormal * -1.0f));
                float2 depthSobel = float2((leftTopDepth01 * -1.0f + leftDepth01 * -2.0f + leftBottomDepth01 * -1.0f + rightTopDepth01 + rightDepth01 * 2.0f + rightBottomDepth01), 
                                           (leftTopDepth01 + topDepth01 * 2.0f + rightTopDepth01 * 1.0f + leftBottomDepth01 * -1.0f + bottomDepth01 * -2.0f + rightBottomDepth01 * -1.0f));
                return float4(normalSobel.x, normalSobel.y, depthSobel * 1.2f);
            }

            float CalculateOutlineSobel(sampler2D tex, float2 pixelCoord, float2 resolution)
            {
                const float4 sobelResult = calculateNormalMapSobel(tex, pixelCoord, resolution);
                const float2 normalSobel = sobelResult.xy;
                const float2 depthSobel = sobelResult.zw;
                const float4 centerDepthNormal = tex2D(_CameraDepthNormalsTexture, pixelCoord / resolution);
                float centerDepth;
                float3 centerNormal;
                DecodeDepthNormal(centerDepthNormal, centerDepth, centerNormal);
                const float normResult =  abs(normalSobel.x) + abs(normalSobel.y);
                const float depthResult = length(depthSobel) - centerDepth;

                return saturate(1.0f - (step(_OutlineNormalThreshold, normResult)
                       + step(_OutlineDepthThreshold, depthResult)));
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
