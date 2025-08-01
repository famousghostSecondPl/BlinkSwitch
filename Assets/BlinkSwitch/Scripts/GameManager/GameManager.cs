namespace BlinkSwitch
{
    using System.Collections.Generic;
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
        //TODO: Refactor this code, because it looks really ugly
        private void Start()
        {
            _PlayerInputManager = GetComponent<PlayerInputManager>();
            _PlayerInputs = new List<PlayerInput>();
            if (PlayersAmount > 1)
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
                    _PlayerInputs.Add(playerInput);
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
                if (playerInput != null)
                {
                    _PlayerInputs.Add(playerInput);
                }
                playerIndex++;
            }
            if(PlayersAmount != _PlayerInputs.Count)
            {
                Debug.LogError("Cannot spawn more players, because unity cannot find free gamepad or keyboard to assign to player");
            }
            PlayersAmount = _PlayerInputs.Count;
            for (playerIndex = 0; playerIndex < PlayersAmount; ++playerIndex)
            {
                var playerInput = _PlayerInputs[playerIndex];
                if (PlayersAmount == 1)
                {
                    playerInput.camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                }
                else if (PlayersAmount == 2)
                {
                    if (playerIndex == 0)
                    {
                        playerInput.camera.rect = new Rect(-0.001f, 0.0f, 0.5f, 1.0f);
                    }
                    else if (playerIndex == 1)
                    {
                        playerInput.camera.rect = new Rect(0.501f, 0.0f, 0.5f, 1.0f);
                    }
                }
                else if (PlayersAmount == 3)
                {
                    if (playerIndex == 0)
                    {
                        playerInput.camera.rect = new Rect(-0.001f, -0.001f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 1)
                    {
                        playerInput.camera.rect = new Rect(0.501f, -0.001f, 0.5f, 0.5f);
                    }
                    else if(playerIndex == 2)
                    {
                        playerInput.camera.rect = new Rect(0.0f, 0.501f, 1.0f, 0.5f);
                    }
                }
                else if (PlayersAmount == 3)
                {
                    if (playerIndex == 0)
                    {
                        playerInput.camera.rect = new Rect(-0.001f, -0.001f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 1)
                    {
                        playerInput.camera.rect = new Rect(0.501f, -0.001f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 2)
                    {
                        playerInput.camera.rect = new Rect(-0.001f, 0.501f, 0.5f, 0.5f);
                    }
                    else if (playerIndex == 3)
                    {
                        playerInput.camera.rect = new Rect(0.501f, 0.501f, 0.5f, 0.5f);
                    }
                }
            }
        }
        #endregion Unity Methods

        #region Public Methods

        #endregion Public Methods

        #region Private Variables
        private PlayerInputManager _PlayerInputManager;
        private List<PlayerInput> _PlayerInputs;
        #endregion Private Variables
    }
}
