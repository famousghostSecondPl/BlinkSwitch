namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class PostProcess : MonoBehaviour
    {
        #region Inspector Variables
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
        [SerializeField] private float _BlurEdgeStrength = 0.05f;
        [SerializeField] private float _CurveStrength = 0.3f;

        [Header("Blood Shader")]
        [SerializeField] private PlayerStats _PlayerStats;
        [SerializeField] private Material _DamageMaterial;
        [SerializeField] private Texture _ScreenSpaceBloodTexture;
        #endregion Inspector Variables

        #region Public Variables
        [HideInInspector] public Camera PlayerCamera;
        [HideInInspector] public PlayerInput PlayerInput;
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
            _DamageTextureResult = TextureUtilities.CreateTextureBilinearClamp(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth);
            _PostProcessIndex = _PostProcessIndex = Random.Range(0, _PostProcessCount * 100) / 100;
        }

        private void Update()
        {
            if (PlayerInput.actions["Blink"].WasPressedThisFrame() && !_Blinking)
            {
                StartCoroutine(Blink());
            }
            _PostProcessEffect = _PostProcessGenerator.GetPostProcessEffectFromId(_PostProcessIndex);
            _PostProcessEffect.Update();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _DamageMaterial.SetFloat(_DamageIndicatorId, _PlayerStats.DamageIndicator);
            _DamageMaterial.SetTexture(_ScreenSpaceBloodTextureId, _ScreenSpaceBloodTexture);
            Graphics.Blit(source, _DamageTextureResult, _DamageMaterial);
            _PostProcessEffect = _PostProcessGenerator.GetPostProcessEffectFromId(_PostProcessIndex);
            _PostProcessEffect.Setup();
            _BlinkMaterial.SetTexture(_PostProcessTextureId, _PostProcessEffect.GeneratePostProcess(_DamageTextureResult));

            _BlinkMaterial.SetFloat(_BlinkId, _BlinkValue);
            _BlinkMaterial.SetInt(_PlayersAmountId, BlinkSwitchInstance.Instance.PlayersAmount);
            _BlinkMaterial.SetInt(_PlayerIndexId, PlayerInput.playerIndex);
            _BlinkMaterial.SetFloat(_BlurEdgeStrengthId, _BlurEdgeStrength);
            _BlinkMaterial.SetFloat(_CurveStrengthId, _CurveStrength);
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
        private readonly int _PlayersAmountId = Shader.PropertyToID("_PlayersAmount");
        private readonly int _PlayerIndexId = Shader.PropertyToID("_PlayerIndex");

        private PostProcessGenerator _PostProcessGenerator;
        private IPostProcessEffect _PostProcessEffect;

        //Eye Blur Shader
        private RenderTexture _HorizontalBlurResultTexture;
        private readonly int _BlurStepsId = Shader.PropertyToID("_BlurSteps");
        private readonly int _BlurStrengthId = Shader.PropertyToID("_BlurStrength");

        //Merged Shader
        private RenderTexture _EyeBlurResult;
        private RenderTexture _DamageTextureResult;

        private readonly int _PostProcessTextureId = Shader.PropertyToID("_PostProcessTexture");

        private readonly int _BlinkId = Shader.PropertyToID("_Blink");
        private readonly int _BlurEdgeStrengthId = Shader.PropertyToID("_BlurEdgeStrength");
        private readonly int _CurveStrengthId = Shader.PropertyToID("_CurveStrength");

        //Blood Texture Screen space after getting damage
        //float _DamageIndicator;
        //sampler2D _ScreenSpaceBloodTexture;
        private readonly int _DamageIndicatorId = Shader.PropertyToID("_DamageIndicator");
        private readonly int _ScreenSpaceBloodTextureId = Shader.PropertyToID("_ScreenSpaceBloodTexture");

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
