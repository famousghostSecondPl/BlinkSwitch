namespace BlinkSwitch
{
    using UnityEngine;
    public enum AntiAliasingType
    {
        FXAA = 0,
        TAA = 1,
        SMAA = 2, // Supported only by Forward rendering
    }

    public enum TAA_Version
    {
        DEFAULT = 0,
        VELOCITY_Y_CO_CG = 1
    }

    [CreateAssetMenu(fileName = "PostProcessDefaultSettings", menuName = "BlinkSwitch/PostProcessDefaultSettings")]
    public class PostProcessDefaultSettings : ScriptableObject
    {
        [Header("Anti Aliasing Algorithm")]
        public AntiAliasingType AntiAliasingType;

        [Header("TAA settings")]
        public Material TemporaryAntiAliasingMaterial;
        public TAA_Version TaaAlgorithmVersion;
        public float TaaJitterFactor;
        public int Upscale;
        public int HaltonSamples;
        public float DepthThreshold;
        public float VelocityFactor;

    }
}
