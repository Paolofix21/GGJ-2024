using Code.Promises;
using UnityEngine;

namespace Code.GameModeUtils.WaveBasedMode {
    public class SpawnPoint : MonoBehaviour, IPointOfInterest {
        #region IPointOfInterest
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        #endregion
    }
}