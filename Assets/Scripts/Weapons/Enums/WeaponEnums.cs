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

}