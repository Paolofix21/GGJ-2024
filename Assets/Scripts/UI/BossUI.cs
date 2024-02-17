using Code.Core;
using Code.Core.MatchManagers;
using Code.EnemySystem.Wakakas;
using Code.GameModeUtils.WaveBasedMode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI {
    public sealed class BossUI : UIBehaviour {
        #region Public Variables
        [Header("References")]
        [SerializeField] private Image m_fillBarImage;
        [SerializeField] private Image m_fillBarSlowImage;
        [SerializeField] private Image m_immuneImage;
        #endregion

        #region Private Variables
        private WaveBasedMatchManager _matchManager;
        private WakakaHealth _health;
        #endregion

        #region Behaviour Callbacks
        protected override void Start() {
            _matchManager = (WaveBasedMatchManager)GameEvents.MatchManager;
            _matchManager.OnBossChanged += OnBossChanged;
            _matchManager.EntityManager.OnFinish += OnBeginBossFight;

            if (_matchManager.Boss)
                OnBossChanged(_matchManager.Boss);

            gameObject.SetActive(false);
            m_immuneImage.gameObject.SetActive(false);
        }

        private void Update() => m_fillBarSlowImage.fillAmount = Mathf.Lerp(m_fillBarSlowImage.fillAmount, m_fillBarImage.fillAmount, Time.deltaTime * 2f);
        #endregion

        #region Private Methods
        private void OnBeginBossFight() {
            if (_health)
                UpdateLife(_health.GetCurrent());
            gameObject.SetActive(true);
        }
        #endregion

        #region Event Methods
        private void OnBossChanged(WaveBasedBossEntity value) {
            if (_health) {
                _health.OnHealthChanged -= UpdateLife;
                _health.OnEnableDisable -= ToggleDamageableState;
            }

            _health = value.GetComponent<WakakaHealth>();
            gameObject.SetActive(_health != null);

            if (!_health)
                return;

            _health.OnHealthChanged += UpdateLife;
            _health.OnEnableDisable += ToggleDamageableState;
            UpdateLife(_health.GetCurrent());
        }

        private void ToggleDamageableState(bool isDamageable) => m_immuneImage.gameObject.SetActive(!isDamageable);

        private void UpdateLife(float percent) => m_fillBarImage.fillAmount = percent;
        #endregion
    }
}