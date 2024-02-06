using FMODUnity;
using UnityEngine;

namespace Code.Weapons {
    [System.Serializable]
    public abstract class FiringLogicBase {
        #region Public Variables
        [SerializeField] protected float m_range;

        [Header("References")]
        [SerializeField] protected EventReference m_soundEventReference;

        [Space]
        [SerializeField] protected Transform m_weaponCamera;
        [SerializeField] protected Transform m_effectOrigin;
        #endregion

        #region Private Variables
        protected Weapon _weapon;
        #endregion

        #region Public Methods
        public void Init(Weapon weapon) => _weapon = weapon;

        public abstract void Shoot(Ammunition ammunition);

        public virtual void Boost() { }
        #endregion

        #region Private Methods
        protected abstract void Effect(Vector3 origin, Vector3 lastPosition);
        #endregion
    }
}