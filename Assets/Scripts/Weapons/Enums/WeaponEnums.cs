using System;

namespace Code.Weapons {

    [System.Serializable]
    public enum WeaponType {
        None = 0,
        Pistol = 1,
        AutoRifle = 2,
        Shotgun = 3,
        Whip = 4,
        Unknown = 5
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

}