Shader "BlinkSwitch/GaussianBlurPostProcessEffect"
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

            int _GaussianBlurSteps;
            float _GaussianBlurStrength;
            float _GaussianBlurSigma;

            float calculateBlurCoeff(float x, float coefficient, float strength)
            {
                return strength * exp(-(x * x)/ (2.0f * coefficient * coefficient)) ;
            }

            float3 Blur(float2 uv, float2 resolution)
            {
    
                float3 col = tex2D(_MainTex, uv / resolution).rgb;
    
                float size = 0.0f;
                float sigma = _GaussianBlurSigma;
                float strength = _GaussianBlurStrength;
    
                int halfBlurSteps = _GaussianBlurSteps/2;
                int x = -halfBlurSteps;
                int y = -halfBlurSteps;
                while(y <= halfBlurSteps && y >= (-halfBlurSteps))
                {
                    float yweight = calculateBlurCoeff(float(y), sigma, strength);
                    while(x <= halfBlurSteps && x >= -halfBlurSteps)
                    {
                        float xweight = calculateBlurCoeff(float(x), sigma, strength);
                        float weight = xweight * yweight;
                        col += tex2D(_MainTex, (uv + float2(float(x) * strength, float(y) * strength)) / resolution).rgb * weight;
                        size += weight;
                        x++;
                    }
                    x = -halfBlurSteps;
                    y++;
                }
                return col / size;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = float4(Blur(i.uv * _ScreenParams.xy, _ScreenParams.xy), 1.0f);
                return col;
            }
            ENDCG
        }
    }
}
