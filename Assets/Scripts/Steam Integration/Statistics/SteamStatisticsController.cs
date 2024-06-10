using System.Collections.Generic;
using Code.Utilities;
using Steamworks;
using UnityEngine;

namespace SteamIntegration.Statistics {
    public delegate void StatisticChangedEventHandler(SteamStatisticSO statistic, float newValue);

    public sealed class SteamStatisticsController : SingletonBehaviour<SteamStatisticsController> {
        #region Public Variables
        [SerializeField] private List<SteamStatisticSO> m_statistics = new();

        public static event StatisticChangedEventHandler OnStatisticChanged;
        #endregion

        #region Overrides
        protected override void OnAfterAwake() {
            if (!SteamUserStats.RequestCurrentStats())
                Debug.LogWarning("[Steamworks.NET] SteamStatsRequestFailedException\nCould not retrieve the stats through the Steam API\n");
        }
        #endregion

        #region Public Methods
        public void AdvanceStat(string id, float amount = 1f) {
            var statistic = m_statistics.Find(a => a.Id == id);
            if (!statistic) {
                Debug.LogWarning($"No statistic with such id was found.\nId: {id}\n", this);
                return;
            }

            AdvanceStat(statistic, amount);
        }

        public void AdvanceStat(string id, int amount = 1) {
            var statistic = m_statistics.Find(a => a.Id == id);
            if (!statistic) {
                Debug.LogWarning($"No statistic with such id was found.\nId: {id}\n", this);
                return;
            }

            AdvanceStat(statistic, amount);
        }

        public void AdvanceStat(SteamStatisticSO statistic, float amount = 1f) {
            if (!statistic) {
                Debug.LogWarning("No statistic was given.\n", this);
                return;
            }

            if (SteamUserStats.GetStat(statistic.Id, out float stat)) {
                if (SteamUserStats.SetStat(statistic.Id, stat + amount)) {
                    SteamUserStats.GetStat(statistic.Id, out float newVal);
                    OnStatisticChanged?.Invoke(statistic, newVal);
                    //Debug.Log($"{statistic.name}: {newVal}\n");
                }
                else
                    Debug.LogWarning("[Steamworks.NET] SteamStatsRequestFailedException\nCould not set a stat through the Steam API\n");
            }
            else
                Debug.LogWarning("[Steamworks.NET] SteamStatsRequestFailedException\nCould not get a stat through the Steam API\n");
        }

        public void AdvanceStat(SteamStatisticSO statistic, int amount = 1) {
            if (!statistic) {
                Debug.LogWarning("No statistic was given.\n", this);
                return;
            }

            if (SteamUserStats.GetStat(statistic.Id, out int stat)) {
                if (SteamUserStats.SetStat(statistic.Id, stat + amount)) {
                    SteamUserStats.GetStat(statistic.Id, out int newVal);
                    OnStatisticChanged?.Invoke(statistic, newVal);
#if UNITY_EDITOR
                    // Debug.Log($"{statistic.name}: {newVal}\n");
#endif
                }
                else
                    Debug.LogWarning("[Steamworks.NET] SteamStatsRequestFailedException\nCould not set a stat through the Steam API\n");
            }
            else
                Debug.LogWarning("[Steamworks.NET] SteamStatsRequestFailedException\nCould not get a stat through the Steam API\n");
        }

        public void PushStats() {
            if (SteamUserStats.StoreStats())
                return;

            Debug.LogError("[Steamworks.NET] SteamStatsRequestFailedException\nCould not store the stats through the Steam API\n");
        }
        #endregion

        #region Public Methods
        #endregion
    }
}