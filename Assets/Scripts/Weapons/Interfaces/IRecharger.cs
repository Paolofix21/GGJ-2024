namespace Code.Weapons {

    public interface IRecharger {
        public WeaponType GetCompatibleWeapon();
        public int GetReloadAmount();
        public void Interactable(bool state);
    }

}