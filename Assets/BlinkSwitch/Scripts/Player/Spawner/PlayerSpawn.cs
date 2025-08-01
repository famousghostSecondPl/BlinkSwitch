namespace BlinkSwitch
{
    using UnityEngine;

    public sealed class PlayerSpawn : MonoBehaviour
    {
        public Vector3 StartPostion => this.transform.position;
        public Quaternion StartRotation => this.transform.rotation;
    }
}
