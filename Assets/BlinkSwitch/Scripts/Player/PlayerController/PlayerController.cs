namespace BlinkSwitch
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerController : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GameObject _RegularBullet;
        [SerializeField] private float _MovementSpeed;
        [SerializeField] private float _RotationSpeed;
        [SerializeField] private float _JumpForce;
        [SerializeField] private float _MaxDistanceFromTheGround = 1.2f;
        #endregion Inspector Variables

        #region Public Variables
        public IPlayerState PlayerState;

        [HideInInspector]
        public Rigidbody Body;
        public Camera MainCamera;
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
            _PlayerStartPoints = FindObjectsByType<PlayerSpawn>(FindObjectsSortMode.None).ToList();
            int randomIndex = Random.Range(0, _PlayerStartPoints.Count - 1);
            transform.position = _PlayerStartPoints[randomIndex].StartPostion;
            transform.rotation = _PlayerStartPoints[randomIndex].StartRotation;
            PlayerState = new PlayerIdle();
            Body = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _PlayerIsShooting = false;
            MainCamera = PlayerInput.camera;
        }


        private void Update()
        {
            if (PlayerState == null)
            {
                PlayerState = new PlayerIdle();
            }
            ClickFire();
            Fire();
            Rotate();
            PlayerState = PlayerState.GetState(this);
            PlayerState.Update(this);
        }

        #endregion Unity Methods

        #region Private Variables
        private float _XAngle;
        private List<PlayerSpawn> _PlayerStartPoints;
        private bool _PlayerIsShooting;
        #endregion Private Variables

        #region Private Methods
        private void Rotate()
        {
            var playerInput = PlayerInput;
            Vector2 look = playerInput.actions["Look"].ReadValue<Vector2>();
            var horizontalAxis = look.x;
            transform.Rotate(horizontalAxis * transform.up * _RotationSpeed * Time.deltaTime);

            var vericalAxis = -look.y;

            _XAngle += vericalAxis * _RotationSpeed * Time.deltaTime;
            _XAngle = Mathf.Clamp(_XAngle, -90.0f, 90.0f);
            MainCamera.transform.localRotation = Quaternion.Euler(Vector3.right * _XAngle);

        }

        private void ClickFire()
        {
            _PlayerIsShooting = PlayerInput.actions["Fire"].WasPressedThisFrame();
        }

        private void Fire()
        {
            if (_PlayerIsShooting)
            {
                var bullet = Instantiate(_RegularBullet, transform.position + MainCamera.transform.forward * 1.5f, Quaternion.identity);
                RegularBullet regularBullet = bullet.GetComponent<RegularBullet>();
                if (bullet != null && regularBullet != null)
                {
                    regularBullet.Fire(MainCamera.transform.forward);
                }
            }
        }
        #endregion Private Methods
    }
}