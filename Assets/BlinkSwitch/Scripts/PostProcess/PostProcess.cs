namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class PostProcess : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Player Input")]

        [Header("Post Process Effects Settings")]
        [SerializeField] private Transform _DirectionalLight;
        [SerializeField] private ComicBookSettings _ComicBookSettings;
        [SerializeField] private SketchEffectSettings _SketchDrawingSettings;
        [SerializeField] private OldTvSettings _OldTvSettings;

        [Header("Eye Blur Shader")]
        [SerializeField] private Material _HorizontalBlurMaterial;
        [SerializeField] private Material _VerticalBlurMaterial;
        [SerializeField] private int _BlurSteps;
        [SerializeField] private float _BlurStrength;
        [SerializeField] private float _BlurSpeedInSeconds;
        [SerializeField] private float _MaxBlurStrength;

        [Header("Blink Shader")]
        [SerializeField] private Material _BlinkMaterial;
        [Range(0.01f, 0.1f)]
        [SerializeField] private float _BlinkingSpeed = 0.01f;
        #endregion Inspector Variables

        #region Public Variables
        public Camera PlayerCamera;
        public PlayerInput PlayerInput;
        #endregion Public Variables

        #region Unity Methods

        private void Start()
        {
            PlayerInput = GetComponent<PlayerInput>();
            PlayerCamera = PlayerInput.camera;

            if (_DirectionalLight == null)
            {
                _DirectionalLight = GameObject.FindWithTag("DirectionalLight").transform;
            }
            PlayerCamera.depthTextureMode = DepthTextureMode.DepthNormals;
            _PostProcessGenerator = new PostProcessGenerator(_ComicBookSettings, _SketchDrawingSettings, _OldTvSettings, PlayerCamera, _DirectionalLight);
            _PostProcessCount = _PostProcessGenerator.GetPostProcessEffectsCounter();

            //TODO: Refactor this part
            StartCoroutine(EyeBluring());
            _EyeBlurResult = TextureUtilities.CreateTextureBilinearClamp(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth);
            _HorizontalBlurResultTexture = TextureUtilities.CreateTextureBilinearClamp(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth);
            _PostProcessIndex = _PostProcessIndex = Random.Range(0, _PostProcessCount * 100) / 100;
        }

        private void Update()
        {
            if (PlayerInput.actions["Blink"].WasPressedThisFrame() && !_Blinking)
            {
                StartCoroutine(Blink());
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _PostProcessEffect = _PostProcessGenerator.GetPostProcessEffectFromId(_PostProcessIndex);
            _PostProcessEffect.Setup();
            _BlinkMaterial.SetTexture(_PostProcessTextureId, _PostProcessEffect.GeneratePostProcess(source));

            _BlinkMaterial.SetFloat(_BlinkId, _BlinkValue);

            
            if (_BlurStrength > 0.0f)
            {
                Graphics.Blit(source, _EyeBlurResult, _BlinkMaterial);
                _HorizontalBlurMaterial.SetFloat(_BlurStrengthId, _BlurStrength);
                _HorizontalBlurMaterial.SetInt(_BlurStepsId, _BlurSteps);
                _VerticalBlurMaterial.SetFloat(_BlurStrengthId, _BlurStrength);
                _VerticalBlurMaterial.SetInt(_BlurStepsId, _BlurSteps);
                Graphics.Blit(_EyeBlurResult, _HorizontalBlurResultTexture, _HorizontalBlurMaterial);
                Graphics.Blit(_HorizontalBlurResultTexture, destination, _VerticalBlurMaterial);
            }
            else
            {
                Graphics.Blit(source, destination, _BlinkMaterial);
            }
        }
        #endregion Unity Methhods

        #region Private Variables
        //Outline shader
        private RenderTexture _OutlineTexture;
        private readonly int _OutlineDepthThresholdId = Shader.PropertyToID("_OutlineDepthThreshold");
        private readonly int _OutlineNormalThresholdId = Shader.PropertyToID("_OutlineNormalThreshold");
        private readonly int _OutlineSizeId = Shader.PropertyToID("_OutlineSize");

        //Dithering Shader
        private readonly int _OutlineTextureId = Shader.PropertyToID("_OutlineTexture");
        private readonly int _PixelSizeId = Shader.PropertyToID("_PixelSize");
        private readonly int _BitsPerColorId = Shader.PropertyToID("_BitsPerColor");
        private readonly int _DitheringThresholdId = Shader.PropertyToID("_DitheringSpreadSize");

        //Old Tv Shader
        private readonly int _CurvatureId = Shader.PropertyToID("_Curvature");
        private readonly int _OldTvPixelSizeId = Shader.PropertyToID("_OldTvPixelSize");
        private readonly int _MinLuminanceThresholdId = Shader.PropertyToID("_MinLuminanceThreshold");
        private readonly int _MaxLuminanceThresholdId = Shader.PropertyToID("_MaxLuminanceThreshold");

        //Pencil Shader
        private RenderTexture _GaussianBlurTexture1;
        private RenderTexture _GaussianBlurTexture2;
        private RenderTexture _DifferenceOfGaussiansTexture;
        private RenderTexture _DogSobelFilterTexture;
        private RenderTexture _PencilEffectTexture;

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

        private PostProcessGenerator _PostProcessGenerator;
        private IPostProcessEffect _PostProcessEffect;

        //Eye Blur Shader
        private RenderTexture _HorizontalBlurResultTexture;
        private readonly int _BlurStepsId = Shader.PropertyToID("_BlurSteps");
        private readonly int _BlurStrengthId = Shader.PropertyToID("_BlurStrength");

        //Merged Shader
        private RenderTexture _EyeBlurResult;
        private RenderTexture _ComicBookPostProcessTetxure;
        private RenderTexture _InvertPostProcessTexture;
        private RenderTexture _OldTvPostProcessTexture;

        private readonly int _PostProcessTextureId = Shader.PropertyToID("_PostProcessTexture");

        private readonly int _BlinkId = Shader.PropertyToID("_Blink");

        private float _BlinkValue;
        private int _PostProcessIndex;
        private int _PostProcessCount;

        private bool _Blinking = false;

        #endregion Private Variables

        #region Private Methods
        private IEnumerator Blink()
        {
            _Blinking = true;
            while(_BlinkValue < 0.6f)
            {
                _BlinkValue += _BlinkingSpeed;
                yield return null;
            }
            _BlurStrength = 0.0f;
            int previousIndex = _PostProcessIndex;
            _PostProcessIndex = Random.Range(0, _PostProcessCount * 100) / 100;
            while(_PostProcessIndex == previousIndex)
            {
                _PostProcessIndex = Random.Range(0, _PostProcessCount * 100) / 100;
            }
            StartCoroutine(OpenEyes());
        }

        private IEnumerator OpenEyes()
        {
            StopCoroutine(Blink());
            while (_BlinkValue > 0.0f)
            {
                _BlinkValue -= _BlinkingSpeed;
                yield return null;
            }
            _Blinking = false;
        }

        private IEnumerator EyeBluring()
        {
            while(true)
            {
                if(_BlurStrength >= _MaxBlurStrength && !_Blinking)
                {
                    StartCoroutine(Blink());
                }
                if (!_Blinking)
                {
                    _BlurStrength += 0.1f;
                }
                yield return new WaitForSeconds(_BlurSpeedInSeconds);
            }
        }
        #endregion Private Methods
    }
}
