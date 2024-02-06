namespace Code.Weapons {
    public interface IRecharger {
        #region Properties
        public WeaponType Type { get; }
        public int Amount { get; }
        #endregion

        #region Public Methods
        public void SetInteractable(bool state);
        #endregion
    }
}