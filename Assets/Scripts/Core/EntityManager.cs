using Code.Promises;
using Code.Utilities;
using UnityEngine;

namespace Code.Core {
    public abstract class EntityManager : MonoBehaviour, IEntityManager {
        #region Public Variables
        #endregion

        #region Private Variables
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        #endregion

        #region IEntityManager
        public SmartCollection<IEntity> Entities { get; } = new();

        public abstract void Enable();

        public abstract void Disable();
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        #endregion
    }
}