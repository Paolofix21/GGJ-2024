using UnityEngine;

namespace Code.Promises {
    public interface IPointOfInterest {
        #region Properties
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        #endregion
    }
}