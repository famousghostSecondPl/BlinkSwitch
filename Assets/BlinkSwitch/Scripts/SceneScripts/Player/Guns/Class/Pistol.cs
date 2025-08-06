namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Pistol : IWeapon
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
            _FireButtonPressed = playerInput.actions["Fire"].WasPressedThisFrame();
        }

        public override void UpdateFire()
        {
            if (_FireButtonPressed && CurrentAmmoInMagazine > 0 && !_IsReloading)
            {
                var bullet = Instantiate(_Bullet, _Position, _Rotation);
                var bulletComponent = bullet.GetComponent<PistolBullet>();
                if (bulletComponent != null)
                {
                    bulletComponent.Fire(_Forward);
                    CurrentAmmoInMagazine--;
                }
            }
        }

        public override bool Reload(PlayerInput playerInput)
        {
            if (!_IsReloading && playerInput.actions["Reload"].WasPressedThisFrame())
            {
                StartCoroutine(Reloading());
            }
            return _IsReloading;
        }

        public override Vector2Int GetCurrentAmmo()
        {
            return new Vector2Int(CurrentAmmoInMagazine, CurrentAmmo);
        }

        #endregion Public Methods

        #region Unity Methods
        private void Start()
        {
            CurrentAmmoInMagazine = _Settings.MaxAmmoInMagazine;
            CurrentAmmo = _Settings.MaxAmmo;
            IsActive = true; // TODO: Add picking up weapon
            _IsReloading = false;
        }
        #endregion Unity Methods

        #region Private Variables
        private Vector3 _Position;
        private Quaternion _Rotation;
        private Vector3 _Forward;
        private bool _FireButtonPressed;
        private bool _IsReloading;
        #endregion Private Variables

        #region Private Methods
        private IEnumerator Reloading()
        {
            _IsReloading = true;
            yield return new WaitForSeconds(_Settings.ReloadingSpeed);
            var ammoDifference = _Settings.MaxAmmoInMagazine - CurrentAmmoInMagazine;
            CurrentAmmoInMagazine =
                CurrentAmmo > _Settings.MaxAmmoInMagazine ? 
                _Settings.MaxAmmoInMagazine 
                : Mathf.Min(_Settings.MaxAmmoInMagazine, CurrentAmmo + CurrentAmmoInMagazine);
            CurrentAmmo -= ammoDifference;
            CurrentAmmo = Mathf.Max(CurrentAmmo, 0);
            _IsReloading = false;
        }
        #endregion Private Methods

    }
}