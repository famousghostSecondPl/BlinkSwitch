namespace BlinkSwitch
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MainMenu : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private TMP_Dropdown _DropDownList;
        #endregion Inspector Variables

        #region Unity Methods
        private void Start()
        {
            BlinkSwitchInstance.Instance.PlayersAmount = 1;
            SceneManager.sceneLoaded += BlinkSwitchInstance.Instance.SpawnGameManager;
        }
        #endregion Unity Methods

        #region Public Methods
        public void ChangePlayer()
        {
            BlinkSwitchInstance.Instance.PlayersAmount = _DropDownList.value + 1;
        }

        public void OpenMap(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        #endregion Public Methods

    }
}
