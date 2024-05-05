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

        #region Overrides
        protected override void OnAfterAwake() => SteamStatisticsController.OnStatisticChanged += OnStatChanged;
        protected override void OnBeforeDestroy() => SteamStatisticsController.OnStatisticChanged -= OnStatChanged;
        #endregion

        #region Public Methods
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

            if (SteamUserStats.GetAchievement(achievement.Id, out var unlocked) && !unlocked)
                SteamUserStats.SetAchievement(achievement.Id);
        }
        #endregion

        #region Event Methods
        private void OnStatChanged(SteamStatisticSO statistic, float newValue) {
            var achievements = m_achievements.Where(a => a.LinkedStat == statistic);
            foreach (var achievement in achievements) {
                if (achievement.LinkedStatThreshold < newValue)
                    continue;

                AdvanceAchievement(achievement);
            }
        }
        #endregion
    }
}