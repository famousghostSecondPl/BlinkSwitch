// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "BlinkSwitch/CustomDepthNormalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {

        Name "RenderObjectsColor"
        Tags { "RenderType"="Opaque" }
        LOD 100

        ZWrite On
        ZTest LEqual
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4x4 _CustomViewProjectionMatrix;
            sampler2D _CustomDepthNormalTexture;

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(UNITY_MATRIX_M, v.vertex);
                float4 clipPos = mul(_CustomViewProjectionMatrix, worldPos);
                o.vertex = clipPos;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal =  o.normal = normalize(mul(v.normal, unity_WorldToObject));
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float screenUv = i.screenPos.xy / i.screenPos.w;
                float previousDepth = tex2D(_CustomDepthNormalTexture, screenUv).r;
                float currentDepth = i.screenPos.z / i.screenPos.w;
                float4 col = tex2D(_CustomDepthNormalTexture, screenUv);
                if(currentDepth >= previousDepth)
                {
                    col = float4(i.screenPos.z / i.screenPos.w, i.normal * 2.0f - 1.0f);
                }

                return col;
            }
            ENDCG
        }
    }
}
