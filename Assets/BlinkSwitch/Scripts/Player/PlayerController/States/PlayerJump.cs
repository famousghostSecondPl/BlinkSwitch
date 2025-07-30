namespace BlinkSwitch
{
    using UnityEngine;

    public class PlayerJump : IPlayerState
    {
        public void Update(PlayerController player)
        {
            if (player == null)
            {
                return;
            }
            if (player.IsPlayerOnGround())
            {
                player.Body.AddForce(Vector3.up * player.JumpForce, ForceMode.Impulse);
            }
        }

        public IPlayerState GetState(PlayerController player)
        {
            if (player == null)
            {
                return new PlayerIdle();
            }
            return new PlayerIdle();
        }

        #region Private Methods

        #endregion Private Methods
    }
}
