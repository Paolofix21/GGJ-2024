using System.Collections.Generic;
using System.Linq;
using Code.Utilities;
using SteamIntegration.Statistics;
using Steamworks;
using UnityEngine;

namespace SteamIntegration.Achievements {
    public sealed class SteamAchievementsController : SingletonBehaviour<SteamAchievementsController> {
        #region Public Variables
        [SerializeField] private List<SteamAchievementSO> m_achievements = new();
        #endregion

        #region Private Variables
        private readonly HashSet<SteamAchievementSO> _achievementsUnlocked = new();
        #endregion

        #region Overrides
        protected override void OnAfterAwake() {
            SteamStatisticsController.OnStatisticChanged += OnStatChanged;

            foreach (var achievement in m_achievements) {
                if (SteamUserStats.GetAchievement(achievement.Id, out var unlocked) && unlocked)
                    _achievementsUnlocked.Add(achievement);
            }
        }

        protected override void OnBeforeDestroy() => SteamStatisticsController.OnStatisticChanged -= OnStatChanged;
        #endregion

        #region Public Methods
        public bool IsAchievementUnlocked(SteamAchievementSO achievement) => _achievementsUnlocked.Contains(achievement);

        public void AdvanceAchievement(string id) {
            var achievement = m_achievements.Find(a => a.Id == id);
            if (!achievement) {
                Debug.LogWarning($"No achievement with such id was found.\nId: {id}\n", this);
                return;
            }

            AdvanceAchievement(achievement);
        }

        public void AdvanceAchievement(SteamAchievementSO achievement) {
            if (!achievement) {
                Debug.LogWarning("No achievement was given.\n", this);
                return;
            }

            if (SteamUserStats.GetAchievement(achievement.Id, out var unlocked)) {
                if (unlocked) {
#if UNITY_EDITOR
                    Debug.Log("[Steamworks.NET] Achievement already unlocked\n");
#endif
                    return;
                }

                if (SteamUserStats.SetAchievement(achievement.Id)) {
                    Debug.Log($"[Steamworks.NET] Unlocked achievement: {achievement.name}\n");
                    SteamUserStats.GetAchievementIcon(achievement.Id);
                    SteamUserStats.GetAchievementName((uint)m_achievements.IndexOf(achievement));
                    SteamStatisticsController.Singleton.PushStats();
                    _achievementsUnlocked.Add(achievement);
                }
                else
                    Debug.LogWarning("[Steamworks.NET] SteamAchievementRequestFailedException\nCould not set an achievement through the Steam API\n");
            }
            else {
                Debug.LogWarning("[Steamworks.NET] SteamAchievementRequestFailedException\nCould not get an achievement through the Steam API\n");
            }
        }
        #endregion

        #region Event Methods
        private void OnStatChanged(SteamStatisticSO statistic, float newValue) {
            var achievements = m_achievements.Where(a => a.LinkedStat == statistic);
            foreach (var achievement in achievements) {
                Debug.Log($"[{achievement.name}] {achievement.LinkedStatThreshold} ? {newValue}\n");
                if (newValue < achievement.LinkedStatThreshold)
                    continue;

                Debug.Log("Lavaluva?\n");
                AdvanceAchievement(achievement);
            }
        }
        #endregion
    }
}