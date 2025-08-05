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
            if (player.PlayerInput.actions["Jump"].WasPressedThisFrame())
            {
                return new PlayerJump();
            }
            if (player.PlayerInput.actions["Move"].IsPressed())
            {
                return new PlayerMove();
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
