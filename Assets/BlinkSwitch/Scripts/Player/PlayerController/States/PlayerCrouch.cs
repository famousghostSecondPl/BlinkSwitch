namespace BlinkSwitch
{
    using UnityEngine;

    public class PlayerCrouch : IPlayerState
    {
        public PlayerCrouch()
        {

        }

        public IPlayerState GetState(PlayerController player)
        {
            return new PlayerCrouch();
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
