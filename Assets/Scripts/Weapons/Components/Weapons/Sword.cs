using Code.EnemySystem;
using Code.Player;
using System;
using UnityEngine;

namespace Code.Weapons {

    public class Sword : Weapon {
        public const int energyToRecharge = 10;
        public static int currentEnergy = 10;
        public static Action OnShoot;

        private void Awake() {
            // TODO - Fix charge with kill
            // WaveSpawner.OnEnemyDeath += OnEnergyTaken;
        }

        private void OnDestroy() {
            // TODO - Fix charge with kill
            // WaveSpawner.OnEnemyDeath -= OnEnergyTaken;
        }

        private void OnEnergyTaken() {
            if (cartridge.HasAmmo())
                return;
            if (PlayerController.Singleton.CurrentSelectedWeapon == 4)
                return;
            if(currentEnergy < energyToRecharge)
                currentEnergy++;
            if(currentEnergy == energyToRecharge)
                Recharge(1);
        }

        public override void Shoot()
        {
            currentEnergy = 0;
            OnShoot.Invoke();
            base.Shoot();
        }
    }

}