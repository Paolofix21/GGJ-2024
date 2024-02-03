using UnityEngine;

namespace Code.Core.MatchManagers {
    public sealed class WaveBasedEntityManager : EntityManager {
        #region Public Variables
        #endregion

        #region Private Variables
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Start() => WaveBasedMatchManager.Singleton.SetEntityManager(this);
        #endregion

        #region Overrides
        public override void Enable() {
            Entities.ForeEach(e => e.Enable());
        }

        public override void Disable() {
            Entities.ForeEach(e => e.Disable());
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        #endregion
    }
}