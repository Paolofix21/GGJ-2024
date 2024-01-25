using System;
using UnityEngine;

namespace Code.Weapons {

    public class Cartridge : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private int totalAmount = default;

        private int currentAmount = default;

        public event Action<int> OnAmmoAmountChanged = default;

        public void AddAmmo(int amount) {
            currentAmount = currentAmount + amount > totalAmount ? totalAmount : currentAmount + amount;
            OnAmmoAmountChanged?.Invoke(currentAmount);
        }

        public void RemoveAmmo(int amount) {
            currentAmount = amount > currentAmount ? 0 : currentAmount - amount;
            OnAmmoAmountChanged?.Invoke(currentAmount);
        }


    }

}