using JetBrains.Annotations;
using System;
using UnityEngine;

public class PlayerWeaponAnimatorListener : MonoBehaviour {
    public event Action OnAnimatorShootCallback = default;

    [UsedImplicitly]
    private void Shoot() {
        OnAnimatorShootCallback?.Invoke();
    }
}
