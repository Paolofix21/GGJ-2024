using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Weapons {

    public class Sword : Weapon {
        public override bool CanShoot() => firingLogic.CanShoot();
    }

}