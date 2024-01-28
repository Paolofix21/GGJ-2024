using UnityEngine;

namespace Code.Graphics {
    [CreateAssetMenu(menuName = "Color Set", fileName = "New Color Set")]
    public sealed class ColorSetSO : ScriptableObject {
        #region Public Variables
        [field: SerializeField, Range(-180f, 180f)] public float ObjectHue { get; private set; } = 0f;
        [field: SerializeField] public float ObjectSaturation { get; private set; } = 2f;
        [field: SerializeField] public Color TrailColor { get; private set; } = Color.white;
        [field: SerializeField, ColorUsage(true, true)] public Color EmissionColor { get; private set; } = Color.white;
        #endregion
    }
}