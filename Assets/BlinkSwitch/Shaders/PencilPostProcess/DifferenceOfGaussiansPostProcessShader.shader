Shader "BlinkSwitch/DifferenceOfGaussiansPostProcessShader"
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

            sampler2D _GaussianBlurTexture1;
            sampler2D _GaussianBlurTexture2;
            float _Sigma;
            float _Threshold;
            float _U;

            float4 frag (v2f i) : SV_Target
            {
                float3 col =  (1.0f + _Sigma) * tex2D(_GaussianBlurTexture2, i.uv).rgb - _Sigma * tex2D(_GaussianBlurTexture1, i.uv).rgb;

                float colLen = length(col);
    
                float val = lerp(1.0f + tanh(_U * (colLen - _Threshold)), 1.0f, step(_Threshold, colLen));
                return float4(val, val, val, 1.0f);
            }
            ENDCG
        }
    }
}
