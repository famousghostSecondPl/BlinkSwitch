namespace BlinkSwitch
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerController : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private float _MovementSpeed;
        [SerializeField] private float _RotationSpeed;
        [SerializeField] private float _JumpForce;
        [SerializeField] private float _MaxDistanceFromTheGround = 1.2f;
        #endregion Inspector Variables

        #region Public Variables
        public IPlayerState PlayerState;

        [HideInInspector]
        public Rigidbody Body;
        public Camera _MainCamera;
        public PlayerInput PlayerInput;
        public float MovementSpeed
        {
            get => _MovementSpeed;
            set
            {
                if (value != _MovementSpeed)
                {
                    _MovementSpeed = value;
                }
            }
        }

        public float RotationSpeed
        {
            get => _RotationSpeed;
            set
            {
                if (value != _RotationSpeed)
                {
                    _RotationSpeed = value;
                }
            }
        }

        public float JumpForce
        {
            get => _JumpForce;
            set
            {
                if (value != _JumpForce)
                {
                    _JumpForce = value;
                }
            }
        }
        #endregion Public Variables

        #region Public Methods
        public bool IsPlayerOnGround()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _MaxDistanceFromTheGround))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        #endregion Public Methods

        #region Unity Methods
        private void Start()
        {
            PlayerState = new PlayerIdle();
            Body = GetComponent<Rigidbody>();
            PlayerInput = GetComponent<PlayerInput>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (PlayerState == null)
            {
                PlayerState = new PlayerIdle();
            }
            Rotate();
            PlayerState = PlayerState.GetState(this);
            PlayerState.Update(this);
        }

        #endregion Unity Methods

        #region Private Variables
        private float _XAngle;
        #endregion Private Variables

        #region Private Methods
        private void Rotate()
        {
            var playerInput = GetComponent<PlayerInput>();
            Vector2 look = playerInput.actions["Look"].ReadValue<Vector2>();
            var horizontalAxis = look.x;
            transform.Rotate(horizontalAxis * transform.up * _RotationSpeed * Time.deltaTime);

            var vericalAxis = -look.y;

            _XAngle += vericalAxis * _RotationSpeed * Time.deltaTime;
            _XAngle = Mathf.Clamp(_XAngle, -90.0f, 90.0f);
            _MainCamera.transform.localRotation = Quaternion.Euler(Vector3.right * _XAngle);

        }
        #endregion Private Methods
    }
}