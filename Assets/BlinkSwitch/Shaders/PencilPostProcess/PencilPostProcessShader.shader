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
            float _LineColorStrength;
            float _SketchLinesStrength;
            sampler2D _CameraDepthTexture;

            float4x4 _MainLightDirectionMatrix;

            float _Sketch1LineSize;
            float _Sketch2LineSize;
            float _Sketch1Threshold;
            float _SketchSkyStrength;
            float _SketchSkyTextureSize;

            float2 rotateUv(float2 uv, float angle)
            {
                return float2(cos(angle) * uv.x - sin(angle) * uv.y, sin(angle) * uv.x + cos(angle) * uv.y);
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 rayOrigin = _WorldSpaceCameraPos.xyz;

                float3 viewVector = mul(unity_CameraInvProjection, float4(i.uv * 2.0f - 1.0f, 0.0f, -1.0f));
                viewVector = mul(unity_CameraToWorld, float4(viewVector, 0.0f));

                float3 rayDirection = normalize(viewVector);
                float cameraDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r) * length(viewVector);

                float3 worldPos = rayOrigin + rayDirection * cameraDepth;

                float2 lightUv = mul(float4(worldPos, 1.0f), _MainLightDirectionMatrix).xy;

                float3 pencilValue = tex2D(_MainTex, i.uv).rgb;

                float dogValue = tex2D(_DogWithoutFilterTexture, i.uv).r;
                float sketch1 = tex2D(_SketchTexture, lightUv * _Sketch1LineSize).r;
                float sketch2 = tex2D(_SketchTexture, (lightUv + float2(0.8f, 0.2f)) * _Sketch2LineSize).r;
                float4 sketchScreenSpace = lerp(float4(1.0f, 1.0f, 1.0f, 1.0f), tex2D(_SketchTexture, rayDirection * _SketchSkyTextureSize) * _SketchSkyStrength, step(0.001f, _SketchSkyStrength));
                float lerpValue = 1.0f - dogValue;

                float sketchColorResult = 1.0f - lerp(sketch1, sketch2, 1.0f - smoothstep(0.0f, _Sketch1Threshold, lerpValue));

                float4 result = lerp(float4(1.0f, 1.0f, 1.0f, 1.0f), float4(0.2f, 0.2f, 0.2f, 1.0f), lerpValue * sketchColorResult * _SketchLinesStrength);
                result = lerp(result, sketchScreenSpace, step(_ProjectionParams.z - 1.0f, cameraDepth));
                float lineColorStrength = 1.0f - _LineColorStrength;
                result = lerp(float4(lineColorStrength, lineColorStrength, lineColorStrength, 1.0f), result, (1.0f - pencilValue.r * sketchColorResult * _LineStrength) );
                return result;
            }
            ENDCG
        }
    }
}
