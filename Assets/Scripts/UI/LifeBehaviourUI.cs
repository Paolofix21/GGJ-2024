using Code.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Code.UI
{
    public class LifeBehaviourUI : MonoBehaviour
    {
        #region Public Variables
        [SerializeField] private Image m_filler;
        #endregion

        #region Properties
        #endregion

        #region Private Variables
        #endregion

        #region Behaviour Callbacks
        private void Start()
        {
            PlayerController.Singleton.Health.OnDamageTaken += UpdateLife;
            PlayerController.Singleton.Health.OnHeal += UpdateLife;
            
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void UpdateLife(float _currentAmount, float _maxAmount)
        {
            float percentage = _currentAmount / _maxAmount;
            m_filler.fillAmount = percentage;
        }
        #endregion

        #region Virtual Methods
        #endregion
    }
}
