using Code.EnemySystem.Boss;
using Code.EnemySystem.Wakakas;
using Code.Weapons;
using UnityEngine;

namespace Utilities {
    public enum DamageObject {
        None = -1,
        Wakaka,
        FireBall,
        LaserBeam,
        Trapezio,
        Bullet,
        Pistol,
        AutomaticRifle,
        Shotgun,
        Whip,
        Sword,
        Unknown
    }

    public static class DamageObjectHelper {
        public static DamageObject Parse(GameObject target) {
            if (target == null)
                return DamageObject.Unknown;

            if (target.TryGetComponent(out Weapon weapon)) {
                return weapon switch {
                    Pistol => DamageObject.Pistol,
                    AutomaticRifle => DamageObject.AutomaticRifle,
                    Shotgun => DamageObject.Shotgun,
                    Whip => DamageObject.Whip,
                    Sword => DamageObject.Sword,
                    _ => DamageObject.Unknown
                };
            }

            if (target.TryGetComponent(out WakakaAttacker _))
                return DamageObject.Wakaka;

            if (target.TryGetComponent(out FireBall fireBall))
                return fireBall.IsTrapezio ? DamageObject.Trapezio : DamageObject.FireBall;

            if (target.TryGetComponent(out WakakaBullet _))
                return DamageObject.Bullet;

            if (target.TryGetComponent(out LaserBeam _))
                return DamageObject.LaserBeam;

            return DamageObject.Unknown;
        }
    }
}