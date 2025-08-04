namespace BlinkSwitch
{
    using UnityEngine;

    public sealed class MoveObject : MonoBehaviour
    {
        #region Inspector Variables
        [Range(0.0f, 1.0f)]
        [SerializeField] private float _MoveValue;
        [SerializeField] private Transform _StarPostion;
        [SerializeField] private Transform _EndPosition;
        #endregion Inspector Variables
        #region Unity Methods
        void Start()
        {
            _Sign = 1.0f;
        }

        void Update()
        {
            _MoveValue += 0.01f * _Sign;
            transform.position = Lerp(_StarPostion.position, _EndPosition.position, Mathf.Pow(_MoveValue, 4.0f));
            if(_MoveValue >= 1.0f)
            {
                _Sign = -1.0f;
            }
            else if(_MoveValue <= 0.0f)
            {
                _Sign = 1.0f;
            }
        }
        #endregion Unity Methods

        private float _Sign;

        #region Private Methods
        private Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return (b - a) * t + a;
        }
        #endregion Private Methods
    }
}
