namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;

    public sealed class Rocket : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private RocketLauncherSettings _Settings;
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
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                var playerStats = other.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    float distance = (playerStats.transform.position - this.transform.position).magnitude;
                    playerStats.RegisterDamage(_Settings.Damage * (_Settings.ExplosionStrength / Mathf.Max(1.0f, distance * distance)));
                }
                else
                {
                    Debug.LogError("[Rocket]: Cannot give damage to player");
                }
            }
            var body = other.GetComponent<Rigidbody>();
            if (body != null)
            {
                Vector3 dir = body.transform.position - this.transform.position;
                body.AddForce(dir.normalized * _Settings.ExplosionStrength / Mathf.Max(1.0f, dir.magnitude * dir.magnitude), ForceMode.Impulse);
            }
            Destroy(this.gameObject);
        }

        #endregion Unity Methods

        #region Private Variables

        #endregion Private Variables

        #region Private Methods

        #endregion Private Methods
    }
}
