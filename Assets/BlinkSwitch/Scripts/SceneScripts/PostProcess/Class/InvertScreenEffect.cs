namespace BlinkSwitch
{
    using UnityEngine;

    public sealed class InvertScreenEffect : IPostProcessEffect
    {
        #region Public Methods
        public InvertScreenEffect(in Camera camera)
        {
            _Camera = camera;
            _InvertScreenMaterial = new Material(Shader.Find("BlinkSwitch/InvertPostProcess"));
            InitTextures();
        }

        public RenderTexture GeneratePostProcess(RenderTexture source)
        {
            if(_ResultTexture == null)
            {
                return source;
            }
            Graphics.Blit(source, _ResultTexture, _InvertScreenMaterial);
            return _ResultTexture;
        }

        public void Update()
        {

        }

        public void Setup()
        {
            //intentionally left empty
        }

        public void Refresh()
        {
            InitTextures();
        }
        #endregion Public Methods

        #region Private Variables
        private Camera _Camera;

        //Materials
        private Material _InvertScreenMaterial;

        //Textures
        private RenderTexture _ResultTexture;
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
