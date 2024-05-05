using UnityEngine;

namespace Miscellaneous {
    public sealed class JumpPad : MonoBehaviour {
        #region Public Variables
        [field: SerializeField] public float PushSpeed { get; private set; } = 4f;
        #endregion

        #region Behaviour Callbacks
        /*
        private void Awake() {
            if (TryGetComponent(out Rigidbody body))
                return;

            body = gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
        }
        */
        #endregion
    }
}