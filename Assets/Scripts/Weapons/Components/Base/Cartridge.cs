using System;
using UnityEngine;

namespace Code.Weapons {

    public class Cartridge : MonoBehaviour, IRechargeable, IConsumable {
        [Header("Settings")]
        [SerializeField] private int startingAmount = default;
        [SerializeField] private int totalAmount = default;
        [SerializeField] private int amountShotConsumed = default;

        private int currentAmount = default;
        public int TotalAmount { get { return totalAmount; } }
        public int CurrentAmount { get { return currentAmount; } }
        public event Action<int> OnAmmoAmountChanged = default;

        private void Start() {
            AddAmmo(startingAmount);
        }

        public bool HasAmmo() { return currentAmount % amountShotConsumed < currentAmount; }

        public void AddAmmo(int amount) {
            currentAmount = Mathf.Clamp(currentAmount + amount, 0, totalAmount);
            OnAmmoAmountChanged?.Invoke(currentAmount);

            Debug.Log($"{gameObject.name} - {nameof(AddAmmo)} - Current amount {currentAmount}");
        }

        public void RemoveAmmo(int amount) {
            currentAmount = Mathf.Clamp(currentAmount - amount, 0, totalAmount);
            OnAmmoAmountChanged?.Invoke(currentAmount);

            Debug.Log($"{gameObject.name} - {nameof(RemoveAmmo)} - Current amount {currentAmount}");
        }

        public void Consume() {
            if (HasAmmo()) RemoveAmmo(amountShotConsumed);
        }
    }

}