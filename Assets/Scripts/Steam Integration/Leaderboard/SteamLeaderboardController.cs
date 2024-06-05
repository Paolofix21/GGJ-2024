using Code.Utilities;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SteamIntegration.Leaderboard {
    internal struct Leaderboard {
        #region Public Variables
        public SteamLeaderboard_t Board { get; set; }
        public string Name { get; set; }
        #endregion
    }

    public class SteamLeaderboardController : SingletonBehaviour<SteamLeaderboardController> {
        #region Public Variables
        [SerializeField] private List<SteamLeaderboardSO> m_leaderboards = new();
        #endregion

        #region Properties
        #endregion

        #region Private Variables
        private readonly List<CallResult<LeaderboardFindResult_t>> _leaderboardFindResultCallbacks = new();
        private readonly List<CallResult<LeaderboardScoreUploaded_t>> _leaderboardScoreUploadResultCallbacks = new();

        private readonly Dictionary<string, Leaderboard> _leaderboards = new();
        #endregion

        #region Events
        #endregion

        #region Overrides
        protected override void OnAfterAwake() {
            if (m_leaderboards.Count <= 0) {
                Debug.LogWarning("No leaderboard asset has been assigned\nDisabling...\n", this);
                enabled = false;
                return;
            }

            Initialize();
        }
        #endregion

        #region Behaviour Callbacks
#if UNITY_EDITOR
        private void Update() {
            if (Keyboard.current.iKey.wasPressedThisFrame)
                SetLeaderboardEntry(m_leaderboards[0], 1);
        }
#endif
        #endregion

        #region Public Methods
        public void SetLeaderboardEntry(SteamLeaderboardSO leaderboard, int score) {
            if (!leaderboard) {
                Debug.LogWarning("No leaderboard entry was given.\n", this);
                return;
            }

            if (!_leaderboards.TryGetValue(leaderboard.Id, out var board)) {
                Debug.LogWarning($"[Steamworks.NET] No leaderboard with id '{leaderboard.Id}' was found.\n", this);
                return;
            }

            var apiCall = SteamUserStats.UploadLeaderboardScore(board.Board, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);

            var callback = CallResult<LeaderboardScoreUploaded_t>.Create();
            callback.Set(apiCall, OnScoreUpdateResult);
            _leaderboardScoreUploadResultCallbacks.Add(callback);

            SteamAPI.RunCallbacks();
        }
        #endregion

        #region Private Methods1
        private void Initialize() {
            foreach (var leaderboard in m_leaderboards) {
                Debug.Log($"Iterating leaderboards:\nName: '{leaderboard.name}'\n");

                var apiCall = SteamUserStats.FindOrCreateLeaderboard(leaderboard.Id,
                    leaderboard.SortMethod,
                    ELeaderboardDisplayType.k_ELeaderboardDisplayTypeTimeMilliSeconds);

                if (apiCall == SteamAPICall_t.Invalid) {
                    Debug.LogError("[Steamworks.NET] The API call was invalid\n", this);
                    continue;
                }

                var callback = CallResult<LeaderboardFindResult_t>.Create();
                callback.Set(apiCall, OnLeaderBoardFindResult);
                _leaderboardFindResultCallbacks.Add(callback);

                SteamAPI.RunCallbacks();
            }
        }
        #endregion

        #region Event Methods
        private void OnLeaderBoardFindResult(LeaderboardFindResult_t result, bool didFail) {
            if (didFail) {
                Debug.LogWarning("[Steamworks.NET] Failed to find a leaderboard...\n", this);
                return;
            }

            var lbName = SteamUserStats.GetLeaderboardName(result.m_hSteamLeaderboard);
            Debug.Log($"[Steamworks.NET] Found leaderboard: '{lbName}'\n", this);

            _leaderboards.Add(lbName, new() {
                Board = result.m_hSteamLeaderboard,
                Name = lbName
            });
        }

        private void OnScoreUpdateResult(LeaderboardScoreUploaded_t result, bool didFail) {
            if (didFail) {
                Debug.LogWarning("[Steamworks.NET] Failed to update score for a leaderboard\n", this);
                return;
            }

            var lbName = SteamUserStats.GetLeaderboardName(result.m_hSteamLeaderboard);
            Debug.Log($"[Steamworks.NET] Successfully updated score for a leaderboard\nLeaderboard name: '{lbName}'\n", this);
        }
        #endregion
    }
}