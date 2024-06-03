using UnityEngine;

namespace SteamIntegration.Leaderboard {
    [CreateAssetMenu(menuName = "Steam/Leaderboard", fileName = "New leaderboard")]
    public sealed class SteamLeaderboardSO : ScriptableObject {
        #region Public Variables
        [field: SerializeField] public string Id { get; private set; }
        #endregion
    }
}