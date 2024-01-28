namespace Code.Weapons {

    public class AutomaticRifle : Weapon {
        public override void Shoot() {
            base.Shoot();

            if (Cartridge.CurrentAmount <= 0)
                handler.playerController.PlayShootContinuous(false);
        }
    }

}
