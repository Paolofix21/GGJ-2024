using UnityEngine;

namespace SteamIntegration.Statistics {
    public enum StatisticType {
        Int,
        Float
    }

    [CreateAssetMenu(menuName = "Steam/Statistic", fileName = "New Statistic")]
    public class SteamStatisticSO : ScriptableObject {
        #region Public Variables
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public StatisticType Type { get; private set; }
        #endregion
    }
}