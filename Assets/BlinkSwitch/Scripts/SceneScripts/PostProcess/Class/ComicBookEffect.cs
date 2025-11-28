namespace BlinkSwitch
{
    using UnityEngine;
    using UnityEngine.Rendering;

    public sealed class ComicBookEffect : IPostProcessEffect
    {
        #region Public Methods
        public ComicBookEffect(ComicBookSettings settings, in Camera camera)
        {
            _Settings = settings;
            _Camera = camera;
            _OutlineMaterial = new Material(Shader.Find("BlinkSwitch/OutlineShader"));
            _DitheringMaterial = new Material(Shader.Find("BlinkSwitch/DitheringShader"));
            _RenderCustomDepthNormalMaterial = new Material(Shader.Find("BlinkSwitch/CustomDepthNormalShader"));
            InitTextures();
        }

        public RenderTexture GeneratePostProcess(RenderTexture source)
        {
            if(_ResultTexture == null || _OutlineTexture == null)
            {
                return source;
            }

            if (_RenderCustomDepthNormalMaterial != null && _CustomDepthNormalTexture != null)
            {
                Graphics.Blit(Texture2D.blackTexture, _CustomDepthNormalTexture);
                _RenderDepthNormlaCommandBuffer = new CommandBuffer();
                _RenderDepthNormlaCommandBuffer.name = "Render Custom Depth Normal buffer";
                _RenderDepthNormlaCommandBuffer.SetRenderTarget(_CustomDepthNormalTexture);
                _RenderDepthNormlaCommandBuffer.ClearRenderTarget(true, true, Color.clear);
                _RenderCustomDepthNormalMaterial.SetTexture(_CustomDepthNormalTextureId, _CustomDepthNormalTexture);
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_Camera);

                foreach (Renderer r in GameObject.FindObjectsByType<Renderer>(sortMode: FindObjectsSortMode.None))
                {
                    if (GeometryUtility.TestPlanesAABB(planes, r.bounds))
                    {
                        var mf = r.GetComponent<MeshFilter>();
                        if (r.gameObject != null && mf)
                        {
                            RenderCustomDepthNormalTexture(r.gameObject, mf);
                        }
                    }
                }
                _RenderDepthNormlaCommandBuffer.Dispose();
                _RenderDepthNormlaCommandBuffer = null;
            }

            _OutlineMaterial.SetTexture(_CustomDepthNormalTextureId, _CustomDepthNormalTexture);

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

        //RenderBuffers
        CommandBuffer _RenderDepthNormlaCommandBuffer;

        //Materials
        private Material _OutlineMaterial;
        private Material _DitheringMaterial;
        private Material _RenderCustomDepthNormalMaterial;

        //Textures
        private RenderTexture _OutlineTexture;
        private RenderTexture _ResultTexture;
        private RenderTexture _CustomDepthNormalTexture;

        //Outline shader
        private readonly int _OutlineDepthThresholdId = Shader.PropertyToID("_OutlineDepthThreshold");
        private readonly int _OutlineNormalThresholdId = Shader.PropertyToID("_OutlineNormalThreshold");
        private readonly int _OutlineSizeId = Shader.PropertyToID("_OutlineSize");
        private readonly int _CustomDepthNormalTextureId = Shader.PropertyToID("_CustomDepthNormalTexture");

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
                TextureUtilities.CreateTextureClampPoint(_Settings.OutlineTextureSize, _Settings.OutlineTextureSize, 24, RenderTextureFormat.Default, false);
            _ResultTexture = TextureUtilities.CreateTextureBilinearClamp(_Camera.pixelWidth, _Camera.pixelHeight, 24, RenderTextureFormat.Default, false);
            _CustomDepthNormalTexture = new RenderTexture(_Settings.OutlineTextureSize, _Settings.OutlineTextureSize, 24)
            {
                enableRandomWrite = true,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
                autoGenerateMips = false,
                useMipMap = false,
                format = RenderTextureFormat.ARGBFloat
            };

        }

        private void RenderCustomDepthNormalTexture(GameObject obj, MeshFilter meshFilter)
        {
            _RenderDepthNormlaCommandBuffer.DrawMesh(meshFilter.sharedMesh, obj.transform.localToWorldMatrix, _RenderCustomDepthNormalMaterial);

            Graphics.ExecuteCommandBuffer(_RenderDepthNormlaCommandBuffer);
        }
        #endregion Private Methods
    }
}
