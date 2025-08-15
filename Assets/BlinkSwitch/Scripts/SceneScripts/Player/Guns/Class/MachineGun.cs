namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class MachineGun : IWeapon
    {
        #region Inspector Variables
        [SerializeField] private GameObject _Bullet;
        [SerializeField] private MachineGunSettings _Settings;
        #endregion Inspector Variables

        #region Public Variables
        public bool IsActive;
        public int CurrentAmmoInMagazine;
        public int CurrentAmmo;
        #endregion Public Variables

        #region Public Methods
        public override void Fire(PlayerInput playerInput, Quaternion rotation, Vector3 position, Vector3 forward)
        {
            _Position = position;
            _Rotation = rotation;
            _Forward = forward;

            _FireButtonPressed = playerInput.actions["Fire"].IsPressed();
        }

        public override void UpdateFire()
        {
            if(_FireButtonPressed && !_CoroutineRunning && CurrentAmmoInMagazine > 0 && !_IsReloading)
            {
                StartCoroutine(Shooting());
            }
        }

        public override bool Reload(PlayerInput playerInput)
        {
            if(!_IsReloading 
                && playerInput.actions["Reload"].WasPressedThisFrame() 
                && CurrentAmmoInMagazine < _Settings.MaxAmmoInMagazine)
            {
                StartCoroutine(Reloading());
            }
            return _IsReloading;
        }

        public override Vector2Int GetCurrentAmmo()
        {
            return new Vector2Int(CurrentAmmoInMagazine, CurrentAmmo);
        }

        public override void RestartAmmo()
        {
            CurrentAmmoInMagazine = _Settings.MaxAmmoInMagazine;
            CurrentAmmo = _Settings.MaxAmmo;
        }

        #endregion Public Methods

        #region Unity Methods
        private void Start()
        {
            CurrentAmmoInMagazine = _Settings.MaxAmmoInMagazine;
            CurrentAmmo = _Settings.MaxAmmo;
            IsActive = true; // TODO: Add picking up weapon
            _CoroutineRunning = false;
            _IsReloading = false;
        }

        #endregion Unity Methods

        #region Private Variables
        private bool _FireButtonPressed;
        private Vector3 _Position;
        private Quaternion _Rotation;
        private Vector3 _Forward;
        private bool _CoroutineRunning;
        private bool _IsReloading;
        #endregion Private Variables

        #region Private Methods
        private IEnumerator Shooting()
        {
            _CoroutineRunning = true;
            while (_FireButtonPressed && CurrentAmmoInMagazine > 0 && !_IsReloading)
            {
                var bullet = Instantiate(_Bullet, _Position, _Rotation);
                var bulletComponent = bullet.GetComponent<MachineGunBullet>();
                if(bulletComponent != null)
                {
                    bulletComponent.Fire(_Forward);
                    CurrentAmmoInMagazine--;
                }
                yield return new WaitForSeconds(_Settings.ShootingSpeedInSeconds);
            }
            _CoroutineRunning = false;
        }

        private IEnumerator Reloading()
        {
            _IsReloading = true;
            yield return new WaitForSeconds(_Settings.ReloadingSpeed);
            CurrentAmmoInMagazine = CurrentAmmo > _Settings.MaxAmmoInMagazine ? _Settings.MaxAmmoInMagazine : CurrentAmmo;
            CurrentAmmo -= _Settings.MaxAmmoInMagazine;
            CurrentAmmo = Mathf.Max(CurrentAmmo, 0);
            _IsReloading = false;
        }
        #endregion Private Methods
    }
}
