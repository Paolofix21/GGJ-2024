﻿using UnityEngine;

namespace Code.Graphics {
    [CreateAssetMenu(menuName = "Color Set", fileName = "New Color Set")]
    public sealed class ColorSetSO : ScriptableObject {
        #region Public Variables
        [field: SerializeField, Range(-180f, 180f)] public float ObjectHue { get; private set; } = 0f;
        [field: SerializeField] public Color TrailColor { get; private set; } = Color.white;
        #endregion
    }
}