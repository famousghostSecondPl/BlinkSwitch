namespace BlinkSwitch
{
    using System.Collections;
    using UnityEngine;

    public sealed class RegularBullet : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private float _SpeedPower;
        [SerializeField] private Rigidbody _Body;
        #endregion Inspector Variables;

        #region Public Methods
        public void Fire(Vector3 forward)
        {
            _Body.AddForce(forward * _SpeedPower, ForceMode.Impulse);
        }
        #endregion Public Methods

        #region Unity Methods
        private void Start()
        {
            _IsRunning = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Destroy(this.gameObject);
        }
           
        #endregion Unity Methods

        #region Private Variables
        private bool _IsRunning;
        #endregion Private Variables

        #region Private Methods
        private IEnumerator DestroyThis()
        {
            _IsRunning = true;
            yield return null;
            Destroy(this.gameObject);
            _IsRunning = false;

        }
        #endregion Private Methods
    }
}
