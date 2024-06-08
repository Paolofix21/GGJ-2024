using JetBrains.Annotations;
using System;
using UnityEngine;

public class PlayerWeaponAnimatorListener : MonoBehaviour {
    public event Action OnAnimatorShootCallback = default;
    public event Action<int> OnAnimatorShootSidedCallback = default;

    [UsedImplicitly]
    private void Shoot() {
        if(enabled && Time.timeScale != 0)
            OnAnimatorShootCallback?.Invoke();
    }

    [UsedImplicitly]
    private void ShootSided(int leftRight) {
        if(enabled && Time.timeScale != 0)
            OnAnimatorShootSidedCallback?.Invoke(leftRight);
    }
}