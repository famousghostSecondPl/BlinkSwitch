Shader "BlinkSwitch/SobelFilterShader"
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

            float _SobelFilterSize;

            float2 calculateNormalMapSobel(float2 fragCoord, float2 resolution)
            {

                float2 dir = float2(_SobelFilterSize, _SobelFilterSize);
    
                float2 uvLeft =    (fragCoord + float2(-dir.x, 0.0f)) / resolution;
                float2 uvLeftTop = (fragCoord + float2(-dir.x, dir.y)) / resolution;
                float2 uvTop =     (fragCoord + float2(0.0f, dir.y)) /  resolution;
                float2 uvRightTop =(fragCoord + float2(dir.x, dir.y)) / resolution;
                float2 uvRight =   (fragCoord + float2(dir.x, 0.0f)) /  resolution;
                float2 uvRightBot =(fragCoord + float2(dir.x,-dir.y)) / resolution;
                float2 uvBot =     (fragCoord + float2(0.0f, -dir.y)) / resolution;
                float2 uvLeftBot = (fragCoord + float2(-dir.x, -dir.y)) / resolution;
    
                float leftTop = tex2D(_MainTex, uvLeftTop).r;
                float left = tex2D(_MainTex, uvLeft).r;
                float leftBot = tex2D(_MainTex, uvLeftBot).r;
    
                float rightTop = tex2D(_MainTex, uvRightTop).r;
                float right = tex2D(_MainTex, uvRight).r;
                float rightBot = tex2D(_MainTex, uvRightBot).r;
   
        
                float top = tex2D(_MainTex, uvTop).r;
                float bot = tex2D(_MainTex, uvBot).r;
    
    
                return float2(leftTop * -1.0f + left * -2.0f + leftBot * -1.0f + rightTop + right * 2.0f + rightBot, 
                            leftTop * -1.0f + top * -2.0f + rightTop * -1.0f + leftBot + bot * 2.0f + rightBot);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 sobelResult = calculateNormalMapSobel(i.uv * _ScreenParams.xy, _ScreenParams.xy);
                float sobelLen = length(sobelResult);
                float4 col = float4(sobelLen, sobelResult, 1.0f);
                return col;
            }
            ENDCG
        }
    }
}
