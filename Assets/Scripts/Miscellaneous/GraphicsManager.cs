using Code.UI;
using Code.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Miscellaneous {
    public class GraphicsManager : SingletonBehaviour<GraphicsManager> {
        #region Public Variables
        [SerializeField] private VolumeProfile[] m_volumes;
        #endregion

        #region Private Variables
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        protected override void OnAfterAwake() {
            SettingsUI.OnGraphicsQualityChanged += OnGraphicsQualitySetting;
        }

        protected override void OnBeforeDestroy() {
            SettingsUI.OnGraphicsQualityChanged -= OnGraphicsQualitySetting;
        }
        #endregion

        #region Public Methods
        public VolumeProfile GetVolumeProfile(int qualityIndex) => m_volumes[qualityIndex];
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        private void OnGraphicsQualitySetting(int qualityIndex) => QualitySettings.SetQualityLevel(qualityIndex);
        #endregion
    }
}