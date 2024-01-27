using UnityEngine;

namespace Code.Graphics {
    public class Rotator : MonoBehaviour {
        #region Public Variables
        [SerializeField] private float m_anglesPerSecond = 180f;
        #endregion

        #region Behaviour Callbacks
        private void Update() => transform.Rotate(m_anglesPerSecond * Time.unscaledDeltaTime * Vector3.up, Space.World);
        #endregion
    }
}