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

            float4x4 _CurrentViewProjectionMatrix;
            float4x4 _PreviousViewProjectionMatrix;

            float2 _CurrentFrameJitter;
            float2 _PreviousFrameJitter;

            float4 frag (v2f i) : SV_Target
            {
                float4 currentWorldPos = tex2D(_WorldPosFromDepthTexture, i.uv);
                float4 previousWorldPos = tex2D(_PreviousWorldPositionFromDepth, i.uv);

                float4 currentClipPos = mul(_CurrentViewProjectionMatrix, float4(currentWorldPos.xyz, 1.0f));
                float4 previousClipPos = mul(_PreviousViewProjectionMatrix, float4(previousWorldPos.xyz, 1.0f));
                float4 currentClipPosForPreviousVP =  mul(_PreviousViewProjectionMatrix, float4(currentWorldPos.xyz, 1.0f));

                currentClipPos /= currentClipPos.w;
                previousClipPos /= previousClipPos.w;
                currentClipPosForPreviousVP /= currentClipPosForPreviousVP.w;

                float2 currentNdc = currentClipPos.xy;
                float2 previousNdc = previousClipPos.xy;
                float2 currentNdcForPreviousVP = currentClipPosForPreviousVP.xy;

                float2 velocity = previousNdc - currentNdc;
                velocity *= 0.5 + 0.5f;
                velocity -= _CurrentFrameJitter;
                velocity -= _PreviousFrameJitter;

                float4 col = float4(velocity, currentNdcForPreviousVP);

                return col;
            }
            ENDCG
        }
    }
}