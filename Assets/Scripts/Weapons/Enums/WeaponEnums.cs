namespace Code.Weapons {
    [System.Serializable]
    public enum WeaponType {
        Pistol = 0,
        Shotgun = 2,
        AutoRifle = 1,
        Whip = 3,
        Sword = 4
    }

    [System.Serializable]
    [System.Flags]
    public enum DamageType {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 4,
        All = ~0
    }
}