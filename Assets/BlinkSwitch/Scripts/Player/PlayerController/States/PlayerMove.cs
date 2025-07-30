namespace BlinkSwitch
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerMove : IPlayerState
    {
        #region Public Methods
        public PlayerMove()
        {

        }

        public void Update(PlayerController player)
        {
            if (player == null)
            {
                return;
            }

            HandleMovement(player);
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
        #endregion Public Methods

        #region Private Methods
        public void HandleMovement(PlayerController player)
        {
            Vector2 move = player.PlayerInput.actions["Move"].ReadValue<Vector2>();
            var verticalAxis = move.y;
            var horizontalAxis = move.x;
            var movement = verticalAxis * player.transform.forward + horizontalAxis * player.transform.right;
            player.transform.position += movement * Time.deltaTime * player.MovementSpeed;
        }
        #endregion Private Methods
    }
}
