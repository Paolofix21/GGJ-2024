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
        private WaveBasedBossEntity _boss;
        private WakakaHealth _health;
        #endregion

        #region Behaviour Callbacks
        protected override void Start() {
            GameEvents.OnCutsceneStateChanged += CheckVisible;
            GameEvents.OnPauseStatusChanged += CheckVisible;

            _matchManager = (WaveBasedMatchManager)GameEvents.MatchManager;
            _matchManager.OnBossChanged += OnBossChanged;
            _matchManager.EntityManager.OnFinish += OnBeginBossFight;

            if (_matchManager.Boss)
                OnBossChanged(_matchManager.Boss);

            gameObject.SetActive(false);
            m_immuneImage.gameObject.SetActive(true);
        }

        private void Update() => m_fillBarSlowImage.fillAmount = Mathf.Lerp(m_fillBarSlowImage.fillAmount, m_fillBarImage.fillAmount, Time.deltaTime * 2f);

        protected override void OnDestroy() {
            GameEvents.OnCutsceneStateChanged -= CheckVisible;
            GameEvents.OnPauseStatusChanged -= CheckVisible;
        }
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

            if (_boss)
                _boss.OnTriggered -= CheckVisible;

            _boss = value;
            _health = value.GetComponent<WakakaHealth>();
            gameObject.SetActive(_health != null);

            if (_boss)
                _boss.OnTriggered += CheckVisible;

            if (!_health)
                return;

            _health.OnHealthChanged += UpdateLife;
            _health.OnEnableDisable += ToggleDamageableState;
            UpdateLife(_health.GetCurrent());
        }

        private void ToggleDamageableState(bool isDamageable) {
            if (m_immuneImage)
                m_immuneImage.gameObject.SetActive(!isDamageable);
        }

        private void UpdateLife(float percent) => m_fillBarImage.fillAmount = percent;

        private void CheckVisible(bool _) => gameObject?.SetActive((_boss.IsFighting || _health.enabled) && !GameEvents.IsOnHold);
        #endregion
    }
}