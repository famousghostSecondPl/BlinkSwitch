namespace BlinkSwitch
{
    using UnityEngine;

    public sealed class PlayerStats : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private PlayerStatsSettings _PlayerSettings;
        #endregion Inspector Variables

        #region Public Variables
        public float Health;
        public float Speed;
        public bool IsAlive;
        public int CurrentAmmoInMagazine;
        public int CurrentAmmoInWeapon;
        public bool IsReloading;
        #endregion Public Variables

        #region Public Methods
        public void GetDamage(float damage)
        {
            Health -= damage;
        }

        public void SetSpeed(float speed)
        {
            Speed = speed;
        }

        public void RestartHealth()
        {
            Health = _PlayerSettings.StartHealth;
        }
        #endregion Public Methods

        #region Unity Methods
        private void Awake()
        {
            Health = _PlayerSettings.StartHealth;
            Speed = _PlayerSettings.Speed;
            IsAlive = true;
        }
        #endregion Unity Methods
    }
}
