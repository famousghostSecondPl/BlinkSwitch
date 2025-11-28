namespace BlinkSwitch
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "PostProcessDefaultSettings", menuName = "BlinkSwitch/PostProcessDefaultSettings")]
    public class PostProcessDefaultSettings : ScriptableObject
    {
        [Header("Outline settings")]
        public bool EnableOutline;
        public int OutlineTextureSize;
        public float OutlineDepthThreshold;
        public float OutlineNormalThreshold;
        public float OutlineSize;

        [Header("Difference of gaussians params")]
        public int GaussianTextureSize;
        public int GaussianBlurStep;
        public float GaussianBlurStrength;
        public float Sigma;
        public float Threshold;
        public float UParam;
        [Header("Gaussian Blur-1 params")]
        public float GaussianBlurSigma1;
        [Header("Gaussian Blur-2 params")]
        public float GaussianBlurSigma2;

        [Header("Sobel filter params")]
        public float SobelFilterSize;

        [Header("Pencil effect params")]
        public bool UseDoubleDOG;
        public Texture SketchTexture;
        public float LineStrength;
        public float SketchLinesStrength;
        [Range(0.0f, 1.0f)]
        public float LineColorStrength;
        public float Sketch1LineSize;
        public float Sketch2LineSize;
        public float Sketch1Threshold;
        public float SketchSkyStrength;
        public float SketchSkyTextureSize;
    }
}
