using UnityEngine;

namespace Code.Weapons {

    public class Whip : Weapon {
        public override bool CanShoot() => firingLogic.CanShoot();
    }

}