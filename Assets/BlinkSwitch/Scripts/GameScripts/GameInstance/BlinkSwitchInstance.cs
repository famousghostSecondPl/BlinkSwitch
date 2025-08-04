namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public sealed class BlinkSwitchInstance : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GameObject _GameManager;
        #endregion Inspector Variables

        #region Public Variables
        public static BlinkSwitchInstance Instance;

        public int PlayersAmount;
        public bool AllowKeyboardAssigment;
        #endregion Public Variables

        #region Public Methods
        public void SpawnGameManager(Scene scene, LoadSceneMode mode)
        {
            Instantiate(_GameManager);
        }
        #endregion Public Methods

        #region Unity Methods
        private void Awake()
        {
            InitializeGameInstance();
        }
        #endregion Unity Methods

        #region Private Methods

        private void InitializeGameInstance()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion Private Methods
    }
}