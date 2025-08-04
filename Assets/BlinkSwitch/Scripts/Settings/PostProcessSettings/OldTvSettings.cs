namespace BlinkSwitch
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "OldTvSettings", menuName = "BlinkSwitch/OldTvSettings")]
    public class OldTvSettings : ScriptableObject
    {
        public float Curvature;
        public float OldTvPixelSize;
        [Range(0.0f, 1.0f)]
        public float MinLuminanceThreshold;
        [Range(0.0f, 1.0f)]
        public float MaxLuminanceThreshold;
    }
}
