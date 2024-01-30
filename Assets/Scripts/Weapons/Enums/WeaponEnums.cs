using System;

namespace Code.Weapons {

    [System.Serializable]
    public enum WeaponType {
        Pistol = 0,
        Shotgun = 1,
        AutoRifle = 2,
        Whip = 3,
        Sword = 4
    }

    [System.Serializable]
    [Flags]
    public enum DamageType {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 4,
        Gold = 8,
        White = Red | Blue | Green | Gold
    }

    public static class WeaponHelpers {
        public static int ToWeaponIndex(this DamageType type) {
            if (type.HasFlag(DamageType.White))
                return 4;

            if (type.HasFlag(DamageType.Gold))
                return 8;

            if (type.HasFlag(DamageType.Red))
                return 0;

            if (type.HasFlag(DamageType.Green))
                return 1;

            if (type.HasFlag(DamageType.Blue))
                return 2;

            return 3;
        }

        public static int ToWakakaIndex(this DamageType type) {
            if (type.HasFlag(DamageType.Red))
                return 0;

            if (type.HasFlag(DamageType.Green))
                return 1;

            if (type.HasFlag(DamageType.Blue))
                return 2;

            return 3;
        }
    }
}