namespace BlinkSwitch
{
    using NUnit.Framework;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerController : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private float _RespawnTimeInSeconds;
        [SerializeField] private List<IWeapon> _Weapons;
        [SerializeField] private PlayerStats _Stats;
        [SerializeField] private float _MovementSpeed;
        [SerializeField] private float _RotationSpeed;
        [SerializeField] private float _JumpForce;
        [SerializeField] private float _MaxDistanceFromTheGround = 1.2f;
        #endregion Inspector Variables

        #region Public Variables
        public IPlayerState PlayerState;

        [HideInInspector] public IWeapon CurrentWeapon;
        [HideInInspector] public Rigidbody Body;
        [HideInInspector] public Camera MainCamera;
        [HideInInspector] public PlayerInput PlayerInput;
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
                if (Vector3.Dot(Vector3.up, hit.normal) >= 0.99f)
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
            MainCamera = PlayerInput.camera;
            CurrentWeapon = _Weapons[0];
            _WeaponIndex = 0;
            for(int i = 0; i < 3; ++i)
            {
                if(i == _WeaponIndex)
                {
                    continue;
                }
                _Weapons[i].GetComponent<MeshRenderer>().enabled = false;
            }
            _Weapons[_WeaponIndex].GetComponent<MeshRenderer>().enabled = true;
        }


        private void Update()
        {
            if(_Stats.Health <= 0.0f && _Stats.IsAlive)
            {
                StartCoroutine(Respawn());
            }
            if (PlayerState == null)
            {
                PlayerState = new PlayerIdle();
            }
            SwitchWeapon();
            CurrentWeapon.Fire(
                PlayerInput, 
                MainCamera.transform.rotation, 
                MainCamera.transform.position + MainCamera.transform.forward * 1.5f, 
                MainCamera.transform.forward);
            CurrentWeapon.UpdateFire();
            CurrentWeapon.Reload(PlayerInput);
            Rotate();
            PlayerState = PlayerState.GetState(this);
            PlayerState.Update(this);
        }

        #endregion Unity Methods

        #region Private Variables
        private float _XAngle;
        private List<PlayerSpawn> _PlayerStartPoints;
        private int _WeaponIndex;
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

        private void SwitchWeapon()
        {
            int previousWeaponIndex = _WeaponIndex;
            if (_Weapons.Count < 3)
            {
                Debug.LogError("[PlayerController]: Forget to add all weapon prefabs to player prefab");
            }
            if (PlayerInput.actions["Weapon1"].WasPressedThisFrame())
            {
                _WeaponIndex = 0;
            }
            if (PlayerInput.actions["Weapon2"].WasPressedThisFrame())
            {
                _WeaponIndex = 1;
            }
            if (PlayerInput.actions["Weapon3"].WasPressedThisFrame())
            {
                _WeaponIndex = 2;
            }
            if(previousWeaponIndex == _WeaponIndex)
            {
                return;
            }
            CurrentWeapon = _Weapons[_WeaponIndex];
            for (int i = 0; i < 3; ++i)
            {
                if (i == _WeaponIndex)
                {
                    continue;
                }
                _Weapons[i].GetComponent<MeshRenderer>().enabled = false;
            }
            _Weapons[_WeaponIndex].GetComponent<MeshRenderer>().enabled = true;
        }

        private IEnumerator Respawn()
        {
            _Stats.IsAlive = false;
            _Stats.RestartHealth();
            PlayerInput.enabled = false;
            Body.constraints = RigidbodyConstraints.None;
            yield return new WaitForSeconds(_RespawnTimeInSeconds);
            Body.constraints = RigidbodyConstraints.FreezeRotation;
            int randomIndex = Random.Range(0, _PlayerStartPoints.Count - 1);
            transform.position = _PlayerStartPoints[randomIndex].StartPostion;
            transform.rotation = _PlayerStartPoints[randomIndex].StartRotation;
            PlayerState = new PlayerIdle();
            PlayerInput.enabled = true;
            _Stats.IsAlive = true;
        }
        #endregion Private Methods
    }
}