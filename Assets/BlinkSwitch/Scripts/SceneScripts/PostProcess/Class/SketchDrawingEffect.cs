namespace BlinkSwitch
{
    using UnityEngine;

    public sealed class SketchDrawingEffect : IPostProcessEffect
    {
        #region Public Methods
        public SketchDrawingEffect(SketchEffectSettings settings, in Camera camera, Transform directionalLight)
        {
            _Settings = settings;
            _Camera = camera;
            _DirectionalLight = directionalLight;
            _SobelFilterMaterial = new Material(Shader.Find("BlinkSwitch/SobelFilterShader"));
            _DifferenceOfGaussianMaterial = new Material(Shader.Find("BlinkSwitch/DifferenceOfGaussiansPostProcessShader"));
            _GaussianBlurMaterial = new Material(Shader.Find("BlinkSwitch/GaussianBlurPostProcessEffect"));
            _PencilEffectMaterial = new Material(Shader.Find("BlinkSwitch/PencilPostProcessShader"));
            _OutlineMaterial = new Material(Shader.Find("BlinkSwitch/OutlineShader"));
            InitTextures();
        }

        public RenderTexture GeneratePostProcess(RenderTexture source)
        {
            if(_OutlineTexture == null ||
                _GaussianBlurTexture1 == null || 
               _GaussianBlurTexture2 == null ||
               _DifferenceOfGaussiansTexture == null ||
               _DogSobelFilterTexture == null ||
               _ResultTexture == null)
            {
                return source;
            }

            _GaussianBlurMaterial.SetFloat(_GaussianBlurSigmaId, _Settings.GaussianBlurSigma1);
            Graphics.Blit(source, _GaussianBlurTexture1, _GaussianBlurMaterial);
            _GaussianBlurMaterial.SetFloat(_GaussianBlurSigmaId, _Settings.GaussianBlurSigma2);
            Graphics.Blit(source, _GaussianBlurTexture2, _GaussianBlurMaterial);

            Graphics.Blit(null, _DifferenceOfGaussiansTexture, _DifferenceOfGaussianMaterial);
            Graphics.Blit(_DifferenceOfGaussiansTexture, _DogSobelFilterTexture, _SobelFilterMaterial);

            if (_Settings.UseDoubleDOG)
            {
                _GaussianBlurMaterial.SetFloat(_GaussianBlurSigmaId, _Settings.GaussianBlurSigma1);
                Graphics.Blit(_DifferenceOfGaussiansTexture, _GaussianBlurTexture1, _GaussianBlurMaterial);
                _GaussianBlurMaterial.SetFloat(_GaussianBlurSigmaId, _Settings.GaussianBlurSigma2);
                Graphics.Blit(_DifferenceOfGaussiansTexture, _GaussianBlurTexture2, _GaussianBlurMaterial);

                Graphics.Blit(null, _DifferenceOfGaussiansTexture, _DifferenceOfGaussianMaterial);
            }

            if (_Settings.EnableOutline)
            {
                Graphics.Blit(_DogSobelFilterTexture, _OutlineTexture, _OutlineMaterial);
            }
            if (_Settings.EnableOutline)
            {
                _PencilEffectMaterial.SetTexture(_OutlineTextureId, _OutlineTexture);
            }
            else
            {
                _PencilEffectMaterial.SetTexture(_OutlineTextureId, Texture2D.whiteTexture);
            }
            _PencilEffectMaterial.SetMatrix(_MainLightDirectionMatrixId, _DirectionalLight.localToWorldMatrix);
            _PencilEffectMaterial.SetTexture(_SourceTextureId, source);
            Graphics.Blit(_DogSobelFilterTexture, _ResultTexture, _PencilEffectMaterial);

            return _ResultTexture;
        }
        public void Setup()
        {
            _OutlineMaterial.SetFloat(_OutlineDepthThresholdId, _Settings.OutlineDepthThreshold);
            _OutlineMaterial.SetFloat(_OutlineNormalThresholdId, _Settings.OutlineNormalThreshold);
            _OutlineMaterial.SetFloat(_OutlineSizeId, _Settings.OutlineSize);

            _GaussianBlurMaterial.SetInt(_GaussianBlurStepsId, _Settings.GaussianBlurStep);
            _GaussianBlurMaterial.SetFloat(_GaussianBlurStrengthId, _Settings.GaussianBlurStrength);

            _DifferenceOfGaussianMaterial.SetTexture(_GaussianBlurTexture1Id, _GaussianBlurTexture1);
            _DifferenceOfGaussianMaterial.SetTexture(_GaussianBlurTexture2Id, _GaussianBlurTexture2);
            _DifferenceOfGaussianMaterial.SetFloat(_SigmaId, _Settings.Sigma);
            _DifferenceOfGaussianMaterial.SetFloat(_ThresholdId, _Settings.Threshold);
            _DifferenceOfGaussianMaterial.SetFloat(_UValueId, _Settings.UParam);

            _SobelFilterMaterial.SetFloat(_SobelFilterSizeId, _Settings.SobelFilterSize);

            _PencilEffectMaterial.SetFloat(_Sketch1LineSizeId, _Settings.Sketch1LineSize);
            _PencilEffectMaterial.SetFloat(_Sketch2LineSizeId, _Settings.Sketch2LineSize);
            _PencilEffectMaterial.SetFloat(_Sketch1ThresholdId, _Settings.Sketch1Threshold);
            _PencilEffectMaterial.SetFloat(_SketchSkyStrengthId, _Settings.SketchSkyStrength);
            _PencilEffectMaterial.SetFloat(_SketchSkyTextureSizeId, _Settings.SketchSkyTextureSize);
            _PencilEffectMaterial.SetTexture(_SketchTextureId, _Settings.SketchTexture);
            _PencilEffectMaterial.SetTexture(_DogWithoutSobelFilterTextureId, _DifferenceOfGaussiansTexture);
            _PencilEffectMaterial.SetFloat(_LineStrengthId, _Settings.LineStrength);
            _PencilEffectMaterial.SetFloat(_LineColorStrengthId, _Settings.LineColorStrength);
            _PencilEffectMaterial.SetFloat(_SketchLinesStrengthId, _Settings.SketchLinesStrength);
        }

        public void Refresh()
        {
            InitTextures();
        }
        #endregion Public Methods

        #region Private Variables
        private SketchEffectSettings _Settings;

        private Camera _Camera;
        private Transform _DirectionalLight;

        //Materials
        private Material _SobelFilterMaterial;
        private Material _DifferenceOfGaussianMaterial;
        private Material _GaussianBlurMaterial;
        private Material _PencilEffectMaterial;
        private Material _OutlineMaterial;

        //Textures
        private RenderTexture _OutlineTexture;
        private RenderTexture _GaussianBlurTexture1;
        private RenderTexture _GaussianBlurTexture2;
        private RenderTexture _DifferenceOfGaussiansTexture;
        private RenderTexture _DogSobelFilterTexture;
        private RenderTexture _ResultTexture;

        //Outline shader
        private readonly int _OutlineDepthThresholdId = Shader.PropertyToID("_OutlineDepthThreshold");
        private readonly int _OutlineNormalThresholdId = Shader.PropertyToID("_OutlineNormalThreshold");
        private readonly int _OutlineSizeId = Shader.PropertyToID("_OutlineSize");
        private readonly int _OutlineTextureId = Shader.PropertyToID("_OutlineTexture");

        private readonly int _GaussianBlurTexture1Id = Shader.PropertyToID("_GaussianBlurTexture1");
        private readonly int _GaussianBlurTexture2Id = Shader.PropertyToID("_GaussianBlurTexture2");
        private readonly int _SketchTextureId = Shader.PropertyToID("_SketchTexture");
        private readonly int _DogWithoutSobelFilterTextureId = Shader.PropertyToID("_DogWithoutFilterTexture");

        private readonly int _SigmaId = Shader.PropertyToID("_Sigma");
        private readonly int _ThresholdId = Shader.PropertyToID("_Threshold");
        private readonly int _UValueId = Shader.PropertyToID("_U");
        private readonly int _GaussianBlurSigmaId = Shader.PropertyToID("_GaussianBlurSigma");
        private readonly int _GaussianBlurStrengthId = Shader.PropertyToID("_GaussianBlurStrength");
        private readonly int _GaussianBlurStepsId = Shader.PropertyToID("_GaussianBlurSteps");
        private readonly int _SobelFilterSizeId = Shader.PropertyToID("_SobelFilterSize");
        private readonly int _LineStrengthId = Shader.PropertyToID("_LineStrength");
        private readonly int _MainLightDirectionMatrixId = Shader.PropertyToID("_MainLightDirectionMatrix");
        private readonly int _SourceTextureId = Shader.PropertyToID("_SourceTexture");

        //Sketch Texture Params
        private readonly int _LineColorStrengthId = Shader.PropertyToID("_LineColorStrength");
        private readonly int _Sketch1LineSizeId = Shader.PropertyToID("_Sketch1LineSize");
        private readonly int _Sketch2LineSizeId = Shader.PropertyToID("_Sketch2LineSize");
        private readonly int _Sketch1ThresholdId = Shader.PropertyToID("_Sketch1Threshold");
        private readonly int _SketchSkyStrengthId = Shader.PropertyToID("_SketchSkyStrength");
        private readonly int _SketchSkyTextureSizeId = Shader.PropertyToID("_SketchSkyTextureSize");
        private readonly int _SketchLinesStrengthId = Shader.PropertyToID("_SketchLinesStrength");
        #endregion Private Variables

        #region Private Methods
        private void InitTextures()
        {
            TextureUtilities.ReleaseTexture(_OutlineTexture);
            TextureUtilities.ReleaseTexture(_GaussianBlurTexture1);
            TextureUtilities.ReleaseTexture(_GaussianBlurTexture2);
            TextureUtilities.ReleaseTexture(_DifferenceOfGaussiansTexture);
            TextureUtilities.ReleaseTexture(_DogSobelFilterTexture);
            TextureUtilities.ReleaseTexture(_ResultTexture);

            _OutlineTexture =
                TextureUtilities.CreateTextureBilinearClamp(_Settings.OutlineTextureSize, _Settings.OutlineTextureSize, _Camera.depth);
            _GaussianBlurTexture1 =
                TextureUtilities.CreateTextureBilinearClamp(_Settings.GaussianTextureSize, _Settings.GaussianTextureSize, _Camera.depth);
            _GaussianBlurTexture2 = 
                TextureUtilities.CreateTextureBilinearClamp(_Settings.GaussianTextureSize, _Settings.GaussianTextureSize, _Camera.depth);
            _DifferenceOfGaussiansTexture = 
                TextureUtilities.CreateTextureBilinearClamp(_Settings.GaussianTextureSize, _Settings.GaussianTextureSize, _Camera.depth);
            _DogSobelFilterTexture = 
                TextureUtilities.CreateTextureBilinearClamp(_Settings.GaussianTextureSize, _Settings.GaussianTextureSize, _Camera.depth);

            _ResultTexture = TextureUtilities.CreateTextureBilinearClamp(_Camera.pixelWidth, _Camera.pixelHeight, _Camera.depth);
        }
        #endregion Private Methods
    }
}
