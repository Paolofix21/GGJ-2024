using Code.Utilities;

namespace Code.Promises {
    public interface IEntityManager {
        #region Properties
        public SmartCollection<IEntity> Entities { get; }
        #endregion

        #region Public Methods
        public void Begin();
        public void Enable();
        public void Disable();
        public void End();

        public void AddEntity(IEntity entity) => Entities.Add(entity);
        public void RemoveEntity(IEntity entity) => Entities.Remove(entity);
        #endregion
    }
}