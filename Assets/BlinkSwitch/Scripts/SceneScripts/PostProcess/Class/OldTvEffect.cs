namespace BlinkSwitch
{
    using UnityEngine;

    public sealed class OldTvEffect : IPostProcessEffect
    {
        #region Public Methods
        public OldTvEffect(OldTvSettings settings, in Camera camera)
        {
            _Settings = settings;
            _Camera = camera;
            _OldTvMaterial = new Material(Shader.Find("BlinkSwitch/OldTvPostProcessShader"));
            InitTextures();
        }

        public RenderTexture GeneratePostProcess(RenderTexture source)
        {
            if (_ResultTexture == null)
            {
                return source;
            }
            Graphics.Blit(source, _ResultTexture, _OldTvMaterial);
            return _ResultTexture;
        }

        public void Update()
        {

        }

        public void Setup()
        {
            _OldTvMaterial.SetFloat(_CurvatureId, _Settings.Curvature);
            _OldTvMaterial.SetFloat(_OldTvPixelSizeId, _Settings.OldTvPixelSize);
            _OldTvMaterial.SetFloat(_MinLuminanceThresholdId, _Settings.MinLuminanceThreshold);
            _OldTvMaterial.SetFloat(_MaxLuminanceThresholdId, _Settings.MaxLuminanceThreshold);
        }
        public void Refresh()
        {
            InitTextures();
        }
        #endregion Public Methods

        #region Private Variables
        private OldTvSettings _Settings;

        private Camera _Camera;

        //Materials
        private Material _OldTvMaterial;

        //Textures
        private RenderTexture _ResultTexture;

        //Old Tv Shader
        private readonly int _CurvatureId = Shader.PropertyToID("_Curvature");
        private readonly int _OldTvPixelSizeId = Shader.PropertyToID("_OldTvPixelSize");
        private readonly int _MinLuminanceThresholdId = Shader.PropertyToID("_MinLuminanceThreshold");
        private readonly int _MaxLuminanceThresholdId = Shader.PropertyToID("_MaxLuminanceThreshold");

        #endregion Private Variables

        #region Private Methods
        private void InitTextures()
        {
            TextureUtilities.ReleaseTexture(_ResultTexture);
            _ResultTexture = TextureUtilities.CreateTextureBilinearClamp(_Camera.pixelWidth, _Camera.pixelHeight, _Camera.depth);
        }

        #endregion Private Methods
    }
}
