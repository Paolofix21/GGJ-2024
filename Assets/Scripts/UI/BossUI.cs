using Code.EnemySystem.Boss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    #region Public Variables   
    [SerializeField] private GameObject m_bossGameobj;
    [SerializeField] private Image m_bossLife;
    #endregion

    #region Properties
    #endregion

    #region Private Variables
    #endregion

    #region Behaviour Callbacks
    private void OnDisable()
    {
        // BossBehaviour.OnDamage -= UpdateLife;
        m_bossGameobj.SetActive(false);
    }
    #endregion

    #region Public Methods
    public void Setup()
    {
        // BossBehaviour.OnDamage += UpdateLife;
        m_bossGameobj.SetActive(true);
    }
    public void UpdateLife(float remHp)
    {
        m_bossLife.fillAmount = remHp/100;
    }
    #endregion

    #region Private Methods
    #endregion

    #region Virtual Methods
    #endregion
}
