using Code.Promises;
using Code.Utilities;
using UnityEngine;

namespace Code.Core {
    public abstract class EntityManager : MonoBehaviour, IEntityManager {
        #region IEntityManager
        public SmartCollection<IEntity> Entities { get; } = new();

        public abstract void Begin();
        public abstract void Enable();

        public abstract void Disable();
        public abstract void End();
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            Entities.OnAdded += OnEntityAdded;
            Entities.OnRemoved += OnEntityRemoved;
            Entities.OnCleared += OnEntitiesCleared;

            OnAfterAwake();
        }

        private void OnDestroy() {
            OnBeforeDestroy();

            Entities.OnAdded -= OnEntityAdded;
            Entities.OnRemoved -= OnEntityRemoved;
            Entities.OnCleared -= OnEntitiesCleared;
        }
        #endregion

        #region Public Methods
        public void AddEntity(IEntity entity) {
            Entities.Add(entity);
            entity.OnDestroyed += RemoveEntity;
        }

        public void RemoveEntity(IEntity entity) {
            entity.OnDestroyed -= RemoveEntity;
            Entities.Remove(entity);
        }
        #endregion

        #region Private Methods
        protected virtual void OnAfterAwake() { }
        protected virtual void OnBeforeDestroy() { }

        protected virtual void OnEntityAdded(IEntity element) { }

        protected virtual void OnEntityRemoved(IEntity element) { }

        protected virtual void OnEntitiesCleared() { }
        #endregion
    }
}