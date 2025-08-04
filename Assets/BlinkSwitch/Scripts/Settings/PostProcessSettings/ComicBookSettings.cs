namespace BlinkSwitch
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ComicBookSettings", menuName = "BlinkSwitch/ComicBookSettings")]
    public class ComicBookSettings : ScriptableObject
    {
        #region Public Variables
        [Header("Outline")]
        public int OutlineTextureSize;
        public float OutlineDepthThreshold;
        public float OutlineNormalThreshold;
        public float OutlineSize;

        [Header("Dithering")]
        public float PixelSize;
        public float BitsPerColor;
        public float DitheirngThreshold;
        #endregion Public Variables
    }
}
