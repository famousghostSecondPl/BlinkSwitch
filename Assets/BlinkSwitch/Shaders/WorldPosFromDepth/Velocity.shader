Shader "BlinkSwitch/MotionVectors"
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _WorldPosFromDepthTexture;
            sampler2D _PreviousWorldPositionFromDepth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }


            float4x4 _PreviousViewProjectionMatrix;
            
            float2 _CurrentFrameJitter;
            float2 _PreviousFrameJitter;

            float4 frag (v2f i) : SV_Target
            {
                float4 currentWorldPos = tex2D(_WorldPosFromDepthTexture, i.uv);
                float4 previousWorldPos = tex2D(_PreviousWorldPositionFromDepth, i.uv);

                float4 currentClipPos = mul(UNITY_MATRIX_VP, float4(currentWorldPos.xyz, 1.0f));
                float4 previousClipPos = mul(_PreviousViewProjectionMatrix, float4(previousWorldPos.xyz, 1.0f));
                float4 currentClipPosForPreviousVP =  mul(_PreviousViewProjectionMatrix, float4(currentWorldPos.xyz, 1.0f));

                currentClipPos /= currentClipPos.w;
                previousClipPos /= previousClipPos.w;
                currentClipPosForPreviousVP /= currentClipPosForPreviousVP.w;

                float2 currentUv = (currentClipPos.xy * 0.5f + 0.5f);
                float2 previousUv = (previousClipPos.xy * 0.5f + 0.5f);
                float2 currentUvForPreviousVP = (currentClipPosForPreviousVP.xy * 0.5f + 0.5f);

                float2 currPosWithPrevVPAndUv = currentUvForPreviousVP - i.uv;
                float2 prevCurrVelocity = previousUv - currentUv;

                float4 col = float4(prevCurrVelocity, currPosWithPrevVPAndUv);

                return col;
            }
            ENDCG
        }
    }
}