namespace BlinkSwitch
{
    using System.Collections;
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
        public float DamageIndicator => _DamageIndicator;
        #endregion Public Variables

        #region Public Methods
        public void RegisterDamage(float damage)
        {
            Health -= damage;
            Health = Mathf.Max(Health, 0.0f);
            _DamageIndicator = 1.0f;
            if (!_IsShowDamageCoroutineRunnig)
            {
                StartCoroutine(ShowDamage());
            }
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

        #region Private Variable
        private float _DamageIndicator;
        private bool _IsShowDamageCoroutineRunnig;
        #endregion Private Variable

        #region Private Methods
        private IEnumerator ShowDamage()
        {
            _IsShowDamageCoroutineRunnig = true;
            while (_DamageIndicator > 0.0f)
            {
                yield return new WaitForSeconds(_PlayerSettings.BloodStageShowInSeconds);
                _DamageIndicator -= _PlayerSettings.BloodStageValue;
            }
            _IsShowDamageCoroutineRunnig = false;
        }
        #endregion Private Methods
    }
}
