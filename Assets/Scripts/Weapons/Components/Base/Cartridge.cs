using UnityEngine;

namespace Code.Weapons {
    [System.Serializable]
    public class Cartridge : IRechargeable, IConsumable {
        #region Public Variables
        [SerializeField] private int amountShotConsumed;
        [SerializeField] private int startingAmount;
        [field: SerializeField] public int TotalAmount { get; private set; }

        public event System.Action<int> OnAmmoAmountChanged;
        #endregion

        #region Properties
        public int CurrentAmount { get; private set; }
        #endregion

        #region Public Methods
        public void Init() => AddAmmo(startingAmount);

        public bool HasAmmo() { return CurrentAmount % amountShotConsumed < CurrentAmount; }

        public void AddAmmo(int amount) {
            CurrentAmount = Mathf.Clamp(CurrentAmount + amount, 0, TotalAmount);
            OnAmmoAmountChanged?.Invoke(CurrentAmount);

            // Debug.Log($"{gameObject.name} - {nameof(AddAmmo)} - Current amount {currentAmount}");
        }

        public void RemoveAmmo(int amount) {
            CurrentAmount = Mathf.Clamp(CurrentAmount - amount, 0, TotalAmount);
            OnAmmoAmountChanged?.Invoke(CurrentAmount);

            // Debug.Log($"{gameObject.name} - {nameof(RemoveAmmo)} - Current amount {currentAmount}");
        }

        public void Consume() {
            if (HasAmmo())
                RemoveAmmo(amountShotConsumed);
        }

        public float GetAmmoRate() => (float)CurrentAmount / TotalAmount;
        #endregion
    }
}