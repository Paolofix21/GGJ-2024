using Code.Core;
using Code.EnemySystem.Wakakas;
using Code.Promises;
using UnityEngine;
using Utilities;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBehaviour))]
    public class WaveBasedWakakaEntity : MonoBehaviour, IEntity {
        #region Public Variables
        [SerializeField] private int m_score = 1;
        [SerializeField] private int m_invokeChaseWhenFewerThen = 4;
        #endregion

        #region Private Variables
        private WakakaBehaviour _wakakaBehaviour;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => _wakakaBehaviour = GetComponent<WakakaBehaviour>();

        private void OnDestroy() {
            GameEvents.Score += m_score;
            OnDestroyed?.Invoke(this);
        }
        #endregion

        #region IEntity
        public event DestroyEventHandler<IEntity> OnDestroyed;

        public Transform Transform => transform;

        public void Enable() => _wakakaBehaviour.enabled = true;

        public void Disable() => _wakakaBehaviour.enabled = false;

        public void Aggro() => _wakakaBehaviour.ForceChasePlayer();

        public void Terminate() => _wakakaBehaviour.GetComponent<WakakaMaskAnimator>().AnimateDeath();
        #endregion
    }
}