namespace BlinkSwitch
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Users;

    public sealed class GameManager : MonoBehaviour
    {

        #region Unity Methods
        private void Start()
        {
            _PlayerInputManager = GetComponent<PlayerInputManager>();
            _PlayerInputs = new List<PlayerInput>();
            if (BlinkSwitchInstance.Instance.PlayersAmount > 1)
            {
                _PlayerInputManager.splitScreen = true;
            }
            else
            {
                _PlayerInputManager.splitScreen = false;
            }
            int playerIndex = 0;
            while(playerIndex < BlinkSwitchInstance.Instance.PlayersAmount)
            {
                PlayerInput playerInput = null;
                if(InitializeKeyboardPlayer(playerInput, playerIndex, BlinkSwitchInstance.Instance.AllowKeyboardAssigment))
                {
                    playerIndex++;
                }
                else if(InitializeGamepadPlayer(playerInput, playerIndex))
                {
                    playerIndex++;
                }
                else
                {
                    Debug.LogError($"Cannot assign device to player {playerIndex+1}");
                    break;
                }
            }
            if(BlinkSwitchInstance.Instance.PlayersAmount != _PlayerInputs.Count)
            {
                Debug.LogError("Cannot spawn more players, because unity cannot find free gamepad or keyboard to assign to player");
            }
            BlinkSwitchInstance.Instance.PlayersAmount = _PlayerInputs.Count;
            InitializeViewport();
        }
        #endregion Unity Methods

        #region Private Methods
        private void InitializeViewport()
        {
            for (int playerIndex = 0; playerIndex < BlinkSwitchInstance.Instance.PlayersAmount; ++playerIndex)
            {
                var playerInput = _PlayerInputs[playerIndex];
                if (BlinkSwitchInstance.Instance.PlayersAmount == 1)
                {
                    playerInput.camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                }
                else if (BlinkSwitchInstance.Instance.PlayersAmount == 2)
                {
                    if (playerIndex == 0)
                    {
                        playerInput.camera.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                    }
                    else if (playerIndex == 1)
                    {
                        playerInput.camera.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                    }
                }
                else if (BlinkSwitchInstance.Instance.PlayersAmount == 3)
                {
                    if (playerIndex == 0)
                    {
                        playerInput.camera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 1)
                    {
                        playerInput.camera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 2)
                    {
                        playerInput.camera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                    }
                }
                else if (BlinkSwitchInstance.Instance.PlayersAmount == 3)
                {
                    if (playerIndex == 0)
                    {
                        playerInput.camera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 1)
                    {
                        playerInput.camera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 2)
                    {
                        playerInput.camera.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 3)
                    {
                        playerInput.camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    }
                }
            }
        }

        private bool InitializeKeyboardPlayer(PlayerInput playerInput, int playerIndex, bool allowKeyboard)
        {
            if(allowKeyboard)
            {
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
                if (playerInput == null)
                {
                    return false;
                }
                _PlayerInputs.Add(playerInput);
                return true;
            }
            return false;
        }

        private bool InitializeGamepadPlayer(PlayerInput playerInput, int playerIndex)
        {
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
            if (playerInput == null)
            {
                return false;
            }
            _PlayerInputs.Add(playerInput);
            return true;
        }
        #endregion Private Methods

        #region Private Variables
        private PlayerInputManager _PlayerInputManager;
        private List<PlayerInput> _PlayerInputs;
        #endregion Private Variables
    }
}
