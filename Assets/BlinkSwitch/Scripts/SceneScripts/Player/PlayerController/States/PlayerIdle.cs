namespace BlinkSwitch
{
    using UnityEngine;

    public class PlayerIdle : IPlayerState
    {
        public PlayerIdle()
        {

        }

        public IPlayerState GetState(PlayerController player)
        {
            if (player.PlayerInput.actions["Move"].IsPressed())
            {
                return new PlayerMove();
            }
            if (player.PlayerInput.actions["Move"].WasPressedThisFrame())
            {
                return new PlayerJump();
            }
            return new PlayerIdle();
        }

        public void Update(PlayerController player)
        {
            if (player == null)
            {
                return;
            }
        }
    }
}
