using UnityEngine;

namespace Code.Promises {
    public interface IEntity {
        #region Properties
        public Transform Transform { get; }
        #endregion

        #region Public Methods
        public void Enable();
        public void Disable();
        #endregion
    }
}