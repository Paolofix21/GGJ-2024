using System;
using UnityEngine;

namespace Code.Weapons {

    public class Cartridge : MonoBehaviour, IRechargeable, IConsumable {
        [Header("Settings")]
        [SerializeField] private int totalAmount = default;
        [SerializeField] private int amountShotConsumed = default;

        private int currentAmount = default;

        public event Action<int> OnAmmoAmountChanged = default;

        public bool HasAmmo() { return currentAmount % amountShotConsumed < currentAmount; }

        public void AddAmmo(int amount) {
            currentAmount = Mathf.Clamp(currentAmount + amount, 0, totalAmount);
            OnAmmoAmountChanged?.Invoke(currentAmount);
        }

        public void RemoveAmmo(int amount) {
            currentAmount = Mathf.Clamp(currentAmount - amount, 0, totalAmount);
            OnAmmoAmountChanged?.Invoke(currentAmount);
        }

        public void Consume() {
            if (HasAmmo()) RemoveAmmo(amountShotConsumed);
        }
    }

}