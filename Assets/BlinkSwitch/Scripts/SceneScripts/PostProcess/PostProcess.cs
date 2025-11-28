namespace BlinkSwitch
{
    using NUnit.Framework.Constraints;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public enum AntiAliasingType
    { 
        FXAA = 0,
        TAA = 1,
        SMAA = 2, // Supported only by Forward rendering
    }

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

        [Header("Anti Aliasing")]
        [SerializeField] private AntiAliasingType _AntiAliasingType;
        [SerializeField] private Material _TemporaryAntiAliasingMaterial;

        [Header("G-Buffer")]
        [SerializeField] private Material _WorldPosFromDepthMaterial;
        [SerializeField] private Material _MotionVectorMaterial;
        #endregion Inspector Variables

        #region Public Variables
        [HideInInspector] public Camera PlayerCamera;
        [HideInInspector] public PlayerInput PlayerInput;
        #endregion Public Variables

        #region Unity Methods

        private void Awake()
        {
            _FrameNumber = 0;
        }

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
            _PreviousFrameTexture = TextureUtilities.CreateTextureBilinearClamp(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth, RenderTextureFormat.ARGBFloat, false);
            _CurrentFrameTexture = TextureUtilities.CreateTextureBilinearClamp(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth, RenderTextureFormat.ARGBFloat, false);
            _TemporaryFrameTexture = TextureUtilities.CreateTextureBilinearClamp(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth, RenderTextureFormat.ARGBFloat, false);
            _WorldPosFromDepthTexture = TextureUtilities.CreateTextureClampPoint(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth, RenderTextureFormat.ARGBFloat, false);
            _PreviousWorldPosFromDepthTexture = TextureUtilities.CreateTextureClampPoint(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth, RenderTextureFormat.ARGBFloat, false);
            _MotionVectorsTexture = TextureUtilities.CreateTextureClampPoint(PlayerCamera.pixelWidth, PlayerCamera.pixelHeight, PlayerCamera.depth, RenderTextureFormat.ARGBFloat, false);
            _PostProcessIndex = _PostProcessIndex = Random.Range(0, _PostProcessCount * 100) / 100;

        }

        private void Update()
        {

            if (PlayerInput.actions["Blink"].WasPressedThisFrame() && !_Blinking)
            {
                StartCoroutine(Blink());
            }

        }

        private void OnPreCull()
        {
            if(_AntiAliasingType == AntiAliasingType.TAA)
            {
                _CurrentJitter = GenerateJitter(_FrameNumber) * 2.0f;
                var projectionMatrix = PlayerCamera.projectionMatrix;
                projectionMatrix.m02 += _CurrentJitter.x;
                projectionMatrix.m12 += _CurrentJitter.y;
                PlayerCamera.projectionMatrix = projectionMatrix;
                _PreviousJitter = _CurrentJitter;
            }
        }

        private void OnPostRender()
        {
            PlayerCamera.ResetProjectionMatrix();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(_FrameNumber == 0)
            {
                Graphics.Blit(source, _PreviousFrameTexture);
                Graphics.Blit(source, _CurrentFrameTexture);
            }
            PlayerCamera.ResetProjectionMatrix();
            Graphics.Blit(source, _WorldPosFromDepthTexture, _WorldPosFromDepthMaterial);
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
                Graphics.Blit(_HorizontalBlurResultTexture, _CurrentFrameTexture, _VerticalBlurMaterial);
            }
            else
            {
                Graphics.Blit(source, _CurrentFrameTexture, _BlinkMaterial);
            }
            var previousProjViewMatrix = GL.GetGPUProjectionMatrix(_PreviousProjectionMatrix, false) * _PreviousViewMatrix;
            _MotionVectorMaterial.SetTexture(_WorldPosTextureFromDepthId, _WorldPosFromDepthTexture);
            _MotionVectorMaterial.SetTexture(_PreviousWorldPosFromDepthTextureId, _PreviousWorldPosFromDepthTexture);
            _MotionVectorMaterial.SetMatrix(_PreviousViewProjectionMatrixId, previousProjViewMatrix);
            _MotionVectorMaterial.SetMatrix(_CurrentViewProjectionMatrixId, GL.GetGPUProjectionMatrix(PlayerCamera.projectionMatrix, false) * PlayerCamera.worldToCameraMatrix);
            _MotionVectorMaterial.SetVector(_CurrentFrameJitterId, _CurrentJitter);
            _MotionVectorMaterial.SetVector(_PreviousFrameJitterId, _PreviousJitter);
            Graphics.Blit(source, _MotionVectorsTexture, _MotionVectorMaterial);
            if (_AntiAliasingType == AntiAliasingType.TAA)
            {
                _TemporaryAntiAliasingMaterial.SetVector(_CurrentFrameJitterId, _CurrentJitter);
                _TemporaryAntiAliasingMaterial.SetVector(_PreviousFrameJitterId, _PreviousJitter);
                _TemporaryAntiAliasingMaterial.SetTexture(_MotionVectorTextureId, _MotionVectorsTexture);
                _TemporaryAntiAliasingMaterial.SetMatrix(_PreviousViewProjectionMatrixId, previousProjViewMatrix);
                _TemporaryAntiAliasingMaterial.SetTexture(_PreviousFrameTextureId, _PreviousFrameTexture);
                _TemporaryAntiAliasingMaterial.SetTexture(_WorldPosTextureFromDepthId, _WorldPosFromDepthTexture);
                Graphics.Blit(_CurrentFrameTexture, _TemporaryFrameTexture, _TemporaryAntiAliasingMaterial);
                Graphics.Blit(_TemporaryFrameTexture, _PreviousFrameTexture);
                Graphics.Blit(_TemporaryFrameTexture, destination);
                Graphics.Blit(source, _PreviousWorldPosFromDepthTexture, _WorldPosFromDepthMaterial);
            }
            else
            {
                //TODO: add anti aliasing algorithms (at least FXAA)
                Graphics.Blit(_CurrentFrameTexture, destination);
            }
            _FrameNumber++;
            _PreviousProjectionMatrix = PlayerCamera.projectionMatrix;
            _PreviousViewMatrix = PlayerCamera.worldToCameraMatrix;
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
        private RenderTexture _PreviousFrameTexture;
        private RenderTexture _CurrentFrameTexture;
        private RenderTexture _TemporaryFrameTexture;
        private RenderTexture _WorldPosFromDepthTexture;
        private RenderTexture _PreviousWorldPosFromDepthTexture;
        private RenderTexture _MotionVectorsTexture;

        private readonly int _PostProcessTextureId = Shader.PropertyToID("_PostProcessTexture");

        private readonly int _BlinkId = Shader.PropertyToID("_Blink");
        private readonly int _BlurEdgeStrengthId = Shader.PropertyToID("_BlurEdgeStrength");
        private readonly int _CurveStrengthId = Shader.PropertyToID("_CurveStrength");

        private readonly int _DamageIndicatorId = Shader.PropertyToID("_DamageIndicator");
        private readonly int _ScreenSpaceBloodTextureId = Shader.PropertyToID("_ScreenSpaceBloodTexture");

        //TAA
        private readonly int _PreviousFrameTextureId = Shader.PropertyToID("_PreviousFrameTexture");
        private readonly int _WorldPosTextureFromDepthId = Shader.PropertyToID("_WorldPosFromDepthTexture");
        private Matrix4x4 _PreviousProjectionMatrix;
        private Matrix4x4 _PreviousViewMatrix;

        private Vector2 _CurrentJitter;
        private Vector2 _PreviousJitter;

        //G-Buffer
        private readonly int _PreviousWorldPosFromDepthTextureId = Shader.PropertyToID("_PreviousWorldPositionFromDepth");
        private readonly int _PreviousViewProjectionMatrixId = Shader.PropertyToID("_PreviousViewProjectionMatrix");
        private readonly int _CurrentViewProjectionMatrixId = Shader.PropertyToID("_CurrentViewProjectionMatrix");
        private readonly int _MotionVectorTextureId = Shader.PropertyToID("_MotionVectorsTexture");
        private readonly int _CurrentFrameJitterId = Shader.PropertyToID("_CurrentFrameJitter");
        private readonly int _PreviousFrameJitterId = Shader.PropertyToID("_PreviousFrameJitter");

        private int _FrameNumber;

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

        //Unrea and Unity by default uses this algorithm
        float Halton(int index, int baseValue)
        {
            float result = 0f;
            float fraction = 1f / baseValue;

            while (index > 0)
            {
                result += (index % baseValue) * fraction;
                index /= baseValue;
                fraction /= baseValue;
            }

            return result;
        }

        Vector2 GenerateJitter(int frame)
        {
            float x = Halton(frame % 1024, 2) - 0.5f;
            float y = Halton(frame % 1024, 3) - 0.5f;

            return new Vector2(x / PlayerCamera.pixelWidth,
                               y / PlayerCamera.pixelHeight);
        }

        #endregion Private Methods
    }
}
