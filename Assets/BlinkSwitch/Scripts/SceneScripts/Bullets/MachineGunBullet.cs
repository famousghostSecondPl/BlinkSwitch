namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class MachineGunBullet : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private MachineGunSettings _Settings;
        [SerializeField] private Rigidbody _Body;
        #endregion Inspector Variables;

        #region Public Methods
        public void Fire(Vector3 forward)
        {
            _Body.AddForce(forward * _Settings.Speed, ForceMode.Impulse);
        }
        #endregion Public Methods

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                var playerStats = other.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.GetDamage(_Settings.Damage);
                }
                else
                {
                    Debug.LogError("[MachineGunBullet]: Cannot give damage to player");
                }
            }
            Destroy(this.gameObject);
        }
           
        #endregion Unity Methods
    }
}
