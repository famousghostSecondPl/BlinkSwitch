Shader "BlinkSwitch/WorldPosFromDepth"
{
    Properties
    {
         _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 screenUv = i.screenPos.xy / i.screenPos.w;
                float3 rayOrigin = _WorldSpaceCameraPos.xyz;

                float3 viewVector = mul(unity_CameraInvProjection, float4(i.uv * 2.0f - 1.0f, 0.0f, -1.0f));
                viewVector = mul(unity_CameraToWorld, float4(viewVector, 0.0f));

                float3 rayDirection = normalize(viewVector);
                float cameraDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r) * length(viewVector);

                float3 worldPos = rayOrigin + rayDirection * cameraDepth;

                float4 col = float4(worldPos.xyz, 1.0f);

                return col;
            }
            ENDCG
        }
    }
}