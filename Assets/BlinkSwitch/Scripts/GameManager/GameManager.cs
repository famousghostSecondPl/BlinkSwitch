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
                // 1. Join with Keyboard & Mouse as one player (if not already paired)
                var keyboard = Keyboard.current;
                var mouse = Mouse.current;
                if (keyboard != null && mouse != null &&
                    InputUser.FindUserPairedToDevice(keyboard) == null)
                {
                    playerInput = _PlayerInputManager.JoinPlayer(playerIndex, playerIndex, "Keyboard&Mouse", keyboard);
                    if (playerInput != null)
                        Debug.Log($"Joined player {playerIndex} with keyboard & mouse");
                    else
                        Debug.LogError("JoinPlayer failed for keyboard");
                }
                if (playerInput != null)
                {
                    if (PlayersAmount == 1)
                    {
                        playerInput.camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                    }
                    else if (PlayersAmount == 2)
                    {
                        if (playerIndex == 0)
                        {
                            Debug.Log("Daje pierwszemu playerowi recta");
                            playerInput.camera.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                        }
                        if (playerIndex == 1)
                        {
                            Debug.Log("Daje drugiemu playerowi recta");
                            playerInput.camera.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                        }
                    }
                    playerIndex++;
                    continue;
                }

                foreach (var gamepad in Gamepad.all)
                {
                    if (InputUser.FindUserPairedToDevice(gamepad) != null)
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
                if (PlayersAmount == 1)
                {
                    playerInput.camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                }
                else if (PlayersAmount == 2)
                {
                    if(playerIndex == 0)
                    {
                        playerInput.camera.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                    }
                    if(playerIndex == 1)
                    {
                        playerInput.camera.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                    }
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
