using UnityEngine;

[CreateAssetMenu(fileName = "PistolSettings", menuName = "Guns/PistolSettings")]
public class PistolSettings : ScriptableObject
{
    public bool Auto;
    public float Speed;
    public float Damage;
    public int MaxAmmo;
    public int MaxAmmoInMagazine;
    public float ReloadingSpeed;
    public float PushForce;
}
