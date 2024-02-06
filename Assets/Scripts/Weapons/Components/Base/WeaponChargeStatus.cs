namespace Code.Weapons {
    public class WeaponChargeStatus {
        public string Info { get; set; }

        public event System.Action<WeaponChargeStatus> OnUpdated;

        public void Dispatch() => OnUpdated?.Invoke(this);
    }

    public class WeaponCooldownChargeStatus : WeaponChargeStatus {
        public float CooldownProgress { get; set; }
    }

    public class WeaponEnergyChargeStatus : WeaponChargeStatus {
        public float EnergyAmount { get; set; }
    }

    public class WeaponBulletChargeStatus : WeaponChargeStatus {
        public int CurrentBullets { get; set; }
        public int MaxBullets { get; set; }
    }
}