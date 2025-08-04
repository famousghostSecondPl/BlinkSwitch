namespace BlinkSwitch
{
    public interface IPlayerState
    {
        public void Update(PlayerController player);
        public IPlayerState GetState(PlayerController player);
    }
}
