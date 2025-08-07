using UnityEngine;

[CreateAssetMenu(fileName = "MachineGunSettings", menuName = "Guns/MachineGunSettings")]
public class MachineGunSettings : ScriptableObject
{
    public float ShootingSpeedInSeconds;
    public float Speed;
    public float Damage;
    public int MaxAmmo;
    public int MaxAmmoInMagazine;
    public float ReloadingSpeed;
    public float PushForce;
}
