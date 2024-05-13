using UnityEngine;
using UnityEngine.Events;

namespace Miscellaneous {
    public class TriggerOnDestroy : MonoBehaviour {
        #region Public Variables
        public UnityEvent onDestroy;
        #endregion

        #region Behaviour Callbacks
        private void OnDestroy() => onDestroy?.Invoke();
        #endregion
    }
}