namespace BlinkSwitch
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "SketchEffectSettings", menuName = "BlinkSwitch/SketchEffectSettings")]
    public class SketchEffectSettings : ScriptableObject
    {
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
        public Texture SketchTexture;
        public float LineStrength;
        public float SketchSize;
    }
}
