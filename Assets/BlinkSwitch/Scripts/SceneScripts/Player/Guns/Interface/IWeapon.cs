namespace BlinkSwitch
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public abstract class IWeapon : MonoBehaviour
    {
        public abstract void Fire(PlayerInput playerInput, Quaternion rotation, Vector3 position, Vector3 forward);

        public abstract void UpdateFire();

        public abstract bool Reload(PlayerInput playerInput);

        public abstract Vector2Int GetCurrentAmmo();
    }
}
