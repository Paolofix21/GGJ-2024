using UnityEngine;
using Utilities;

namespace Code.Promises {
    public interface IEntity {
        #region Properties
        public event DestroyEventHandler<IEntity> OnDestroyed;

        public Transform Transform { get; }
        #endregion

        #region Public Methods
        public void Enable();
        public void Disable();
        #endregion
    }
}