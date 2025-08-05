namespace BlinkSwitch
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "RocketLauncherSettings", menuName = "Guns/RocketLauncherSettings")]
    public sealed class RocketLauncherSettings : ScriptableObject
    {
        public float Speed;
        public float Damage;
        public int MaxAmmo;
        public int MaxAmmoInMagazine;
        public float ExplosionStrength;
        public float ShootingSpeed;
        public float ReloadingSpeed;
    }
}
