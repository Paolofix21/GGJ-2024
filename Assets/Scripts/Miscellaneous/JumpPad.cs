using UnityEngine;

namespace Miscellaneous {
    public sealed class JumpPad : MonoBehaviour {
        #region Public Variables
        [field: SerializeField] public float PushSpeed { get; private set; } = 4f;
        #endregion
    }
}