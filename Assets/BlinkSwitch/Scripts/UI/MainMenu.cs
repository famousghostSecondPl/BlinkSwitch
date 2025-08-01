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
            GameManager.PlayersAmount = 1;
        }
        #endregion Unity Methods

        #region Public Methods
        public void ChangePlayer()
        {
            GameManager.PlayersAmount = _DropDownList.value + 1;
        }

        public void OpenMap(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        #endregion Public Method
    }
}
