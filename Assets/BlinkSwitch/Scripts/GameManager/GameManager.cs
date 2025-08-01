namespace BlinkSwitch
{
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Users;

    public sealed class GameManager : MonoBehaviour
    {
        #region Public Variables
        public static int PlayersAmount;
        #endregion Public Variables

        #region Unity Methods
        private void Start()
        {
            _PlayerInputManager = GetComponent<PlayerInputManager>();
            if(PlayersAmount > 1)
            {
                _PlayerInputManager.splitScreen = true;
            }
            else
            {
                _PlayerInputManager.splitScreen = false;
            }
            int playerIndex = 0;
            while(playerIndex < PlayersAmount)
            {
                PlayerInput playerInput = null;
                foreach(var gamepad in Gamepad.all)
                {
                    if(InputUser.FindUserPairedToDevice(gamepad) == null)
                    {
                        continue;
                    }
                    playerInput = _PlayerInputManager.JoinPlayer(playerIndex, playerIndex, "Gamepad", gamepad);
                    if (playerInput != null)
                    {
                        Debug.Log($"Joined player {playerIndex} with {gamepad.displayName}");
                        break;
                    }
                    else
                        Debug.LogError("JoinPlayer failed for gamepad");
                }
                if(playerInput != null)
                {
                    playerIndex++;
                    continue;
                }
                // 2. Join with Keyboard & Mouse as one player (if not already paired)
                var keyboard = Keyboard.current;
                var mouse = Mouse.current;

                if (keyboard != null && mouse != null &&
                    InputUser.FindUserPairedToDevice(keyboard) != null)
                {
                    playerInput = _PlayerInputManager.JoinPlayer(playerIndex, playerIndex, "Keyboard&Mouse", keyboard);
                    if (playerInput != null)
                        Debug.Log($"Joined player {playerIndex} with keyboard & mouse");
                    else
                        Debug.LogError("JoinPlayer failed for keyboard");
                }
                playerIndex++;
            }
        }
        #endregion Unity Methods

        #region Public Methods

        #endregion Public Methods

        #region Private Variables
        private PlayerInputManager _PlayerInputManager;
        #endregion Private Variables
    }
}
