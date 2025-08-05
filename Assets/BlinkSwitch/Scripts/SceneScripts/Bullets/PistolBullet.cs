namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;

    public sealed class PistolBullet : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private PistolSettings _Settings;
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
                    Debug.LogError("[PistolBullet]: Cannot give damage to player");
                }
            }
            Destroy(this.gameObject);
        }
           
        #endregion Unity Methods
    }
}
