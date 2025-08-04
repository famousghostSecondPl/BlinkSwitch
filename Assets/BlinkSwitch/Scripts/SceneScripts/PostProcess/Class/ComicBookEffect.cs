namespace BlinkSwitch
{
    using UnityEngine;

    public sealed class ComicBookEffect : IPostProcessEffect
    {
        #region Public Methods
        public ComicBookEffect(ComicBookSettings settings, in Camera camera)
        {
            _Settings = settings;
            _Camera = camera;
            _OutlineMaterial = new Material(Shader.Find("BlinkSwitch/OutlineShader"));
            _DitheringMaterial = new Material(Shader.Find("BlinkSwitch/DitheringShader"));
            InitTextures();
        }

        public RenderTexture GeneratePostProcess(RenderTexture source)
        {
            if(_ResultTexture == null || _OutlineTexture == null)
            {
                return source;
            }
            //Creating outline texture
            Graphics.Blit(source, _OutlineTexture, _OutlineMaterial);

            //Creating dithering effect
            Graphics.Blit(source, _ResultTexture, _DitheringMaterial);

            return _ResultTexture;
        }

        public void Setup()
        {
            _OutlineMaterial.SetFloat(_OutlineDepthThresholdId, _Settings.OutlineDepthThreshold);
            _OutlineMaterial.SetFloat(_OutlineNormalThresholdId, _Settings.OutlineNormalThreshold);
            _OutlineMaterial.SetFloat(_OutlineSizeId, _Settings.OutlineSize);

            _DitheringMaterial.SetFloat(_PixelSizeId, _Settings.PixelSize);
            _DitheringMaterial.SetFloat(_BitsPerColorId, _Settings.BitsPerColor);
            _DitheringMaterial.SetFloat(_DitheringThresholdId, _Settings.DitheirngThreshold);
            _DitheringMaterial.SetTexture(_OutlineTextureId, _OutlineTexture);
        }

        public void Refresh()
        {
            InitTextures();
        }
        #endregion Public Methods

        #region Private Variables
        private ComicBookSettings _Settings;

        private Camera _Camera;

        //Materials
        private Material _OutlineMaterial;
        private Material _DitheringMaterial;

        //Textures
        private RenderTexture _OutlineTexture;
        private RenderTexture _ResultTexture;

        //Outline shader
        private readonly int _OutlineDepthThresholdId = Shader.PropertyToID("_OutlineDepthThreshold");
        private readonly int _OutlineNormalThresholdId = Shader.PropertyToID("_OutlineNormalThreshold");
        private readonly int _OutlineSizeId = Shader.PropertyToID("_OutlineSize");

        //Dithering Shader
        private readonly int _OutlineTextureId = Shader.PropertyToID("_OutlineTexture");
        private readonly int _PixelSizeId = Shader.PropertyToID("_PixelSize");
        private readonly int _BitsPerColorId = Shader.PropertyToID("_BitsPerColor");
        private readonly int _DitheringThresholdId = Shader.PropertyToID("_DitheringSpreadSize");
        #endregion Private Variables

        #region Private Methods
        private void InitTextures()
        {
            TextureUtilities.ReleaseTexture(_OutlineTexture);
            TextureUtilities.ReleaseTexture(_ResultTexture);
            _OutlineTexture =
                TextureUtilities.CreateTextureClampPoint(_Settings.OutlineTextureSize, _Settings.OutlineTextureSize, _Camera.depth);
            _ResultTexture = TextureUtilities.CreateTextureBilinearClamp(_Camera.pixelWidth, _Camera.pixelHeight, _Camera.depth);
        }
        #endregion Private Methods
    }
}
