Shader "BlinkSwitch/PencilPostProcessShader"
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
                float4 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            sampler2D _SketchTexture;
            sampler2D _DogWithoutFilterTexture;
            sampler2D _SourceTexture;
            float _LineStrength;
            sampler2D _CameraDepthTexture;

            float4x4 _MainLightDirectionMatrix;

            float _SketchLineSize;

            float4 frag (v2f i) : SV_Target
            {
                float3 rayOrigin = _WorldSpaceCameraPos.xyz;

                float3 viewVector = mul(unity_CameraInvProjection, float4(i.uv * 2.0f - 1.0f, 0.0f, -1.0f));
                viewVector = mul(unity_CameraToWorld, float4(viewVector, 0.0f));

                float3 rayDirection = normalize(viewVector);
                float cameraDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r) * length(viewVector);

                float3 worldPos = rayOrigin + rayDirection * cameraDepth;

                float2 lightUv = mul(float4(worldPos, 1.0f), _MainLightDirectionMatrix).xy * _SketchLineSize;

                float pencilValue = tex2D(_MainTex, i.uv).r * _LineStrength;
                float dogValue = tex2D(_DogWithoutFilterTexture, i.uv).r;
                float4 sketchResult = saturate(tex2D(_SketchTexture, lightUv) * tex2D(_SketchTexture, lightUv + float2(0.4f, -0.5f)));
                float4 result = lerp(float4(1.0f, 1.0f, 1.0f, 1.0f), saturate(sketchResult), min(1.0f, pencilValue + (1.0f - dogValue)));
                return result;
            }
            ENDCG
        }
    }
}
