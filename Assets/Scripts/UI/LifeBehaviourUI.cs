using Code.Player;
using Code.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class LifeBehaviourUI : MonoBehaviour {
        #region Public Variables
        [SerializeField] private Image m_filler;
        #endregion

        #region Private Variables
        private PlayerController _target;
        #endregion

        #region Behaviour Callbacks
        private void Start() {
            _target = GameEvents.MatchManager.GetPlayerEntity().Transform.GetComponent<PlayerController>();

            _target.Health.OnDamageTaken += UpdateLife;
            _target.Health.OnHeal += UpdateLife;
        }
        #endregion

        #region Private Methods
        private void UpdateLife(float _currentAmount, float _maxAmount) {
            float percentage = _currentAmount / _maxAmount;
            m_filler.fillAmount = percentage;
        }
        #endregion
    }
}