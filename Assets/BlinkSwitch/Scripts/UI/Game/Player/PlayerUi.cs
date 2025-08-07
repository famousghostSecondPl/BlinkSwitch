namespace BlinkSwitch
{
    using TMPro;
    using UnityEngine;

    public class PlayerUi : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI _AmmonInfo;
        [SerializeField] private TextMeshProUGUI _AmmoUI;
        [SerializeField] private TextMeshProUGUI _HealthUI;
        [SerializeField] private PlayerStats _PlayerStats;
        #endregion Inspector Variables

        #region Unity Methods
        private void Update()
        {
            _AmmoUI.text = _PlayerStats.CurrentAmmoInMagazine.ToString() + "/" + _PlayerStats.CurrentAmmoInWeapon.ToString();
            _HealthUI.text = _PlayerStats.Health.ToString() + " Health";
            if(_PlayerStats.CurrentAmmoInMagazine <= 0 && _PlayerStats.CurrentAmmoInWeapon <= 0)
            {
                _AmmonInfo.text = "No ammo";
                return;
            }
            if (_PlayerStats.CurrentAmmoInMagazine <= 0 && !_PlayerStats.IsReloading)
            {
                _AmmonInfo.text = "Reload";
                return;
            }
            if(_PlayerStats.IsReloading)
            {
                _AmmonInfo.text = "Realoading Wait...";
                return;
            }
            _AmmonInfo.text = "";
        }
        #endregion Unity Methods

        #region Private Variables

        #endregion Private Variables

        #region Private Methods

        #endregion Private Methods
    }
}
