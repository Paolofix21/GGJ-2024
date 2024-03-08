using UnityEngine;

namespace Audio {
    [System.Serializable]
    public sealed class Sound {
        #region Public Variables
        [field: SerializeField] public AudioClip Clip { get; private set; }
        [field: SerializeField] public float Volume { get; private set; }
        #endregion
    }
}