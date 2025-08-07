namespace BlinkSwitch
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "PlayerStatsSettings", menuName = "Player/PlayerStatsSettings")]
    public sealed class PlayerStatsSettings : ScriptableObject
    {
        public float MaxHealth;
        public float StartHealth;
        public float Speed;
        public float RunningSpeed;
        public float BloodStageShowInSeconds;
        public float BloodStageValue;
    }
}
