namespace Code.Weapons {

    public class Sword : Weapon {
        private void Awake() {
            if (firingLogic != null) {
                firingLogic.OnCooldownStateChanged += OnCooldownState;
            }
        }
        private void OnDestroy() {
            if (firingLogic != null) {
                firingLogic.OnCooldownStateChanged -= OnCooldownState;
            }
        }

        // TERRIFICANTE
        private void OnCooldownState(bool state) {
            if (!state)
                Recharge(1);
        }

        public override bool CanShoot() => firingLogic.CanShoot();
    }

}