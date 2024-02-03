using System.Collections.Generic;
using Code.Utilities;

namespace Code.Promises {
    public interface IEntityManager {
        #region Properties
        public SmartCollection<IEntity> Entities { get; }
        #endregion

        #region Public Methods
        public void Enable();
        public void Disable();

        public void AddEntity(IEntity entity) => Entities.Add(entity);
        public void RemoveEntity(IEntity entity) => Entities.Remove(entity);
        public void FillEntities(IEnumerable<IEntity> entity) => Entities.AddRange(entity);
        #endregion
    }
}