using UnityEngine;

namespace Code.Weapons {

    public class Whip : Weapon {

        protected override void Shoot() {
            if (!firingLogic.CanShoot())
                return;

            if (!cartridge.HasAmmo())
                return;

            cartridge.Consume();
            firingLogic.Shoot(ammunition);
        }
    }

}