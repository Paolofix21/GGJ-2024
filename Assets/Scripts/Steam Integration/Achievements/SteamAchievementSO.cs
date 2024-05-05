using SteamIntegration.Statistics;
using UnityEngine;

namespace SteamIntegration.Achievements {
    public enum AchievementType {
        OneTime,
        MultipleTimes,
        Counter,
    }

    [CreateAssetMenu(menuName = "Steam/Achievement", fileName = "New Achievement")]
    public sealed class SteamAchievementSO : ScriptableObject {
        #region Public Variables
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public AchievementType Type { get; private set; }
        [field: SerializeField] public SteamStatisticSO LinkedStat { get; private set; }
        [field: SerializeField] public int LinkedStatThreshold { get; private set; }
        #endregion
    }
}