using Code.Utilities;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SteamIntegration.Leaderboard
{
    public struct SteamLeaderboardData
    {
        public string LeaderboardName;
        public LeaderboardFindResult_t LeaderBoardResult;
    }
    public class SteamLeaderboardController : SingletonBehaviour<SteamLeaderboardController>
    {
        #region Public Variables
        [SerializeField] private List<SteamLeaderboardSO> m_leaderboards = new();
        #endregion

        #region Properties
        #endregion
        #region Private Variables
        private Dictionary<string, SteamLeaderboard_t> _dictLeaderboard;
        private Dictionary<string, SteamLeaderboard_t> _scoresLeaderboard;
        private SteamLeaderboardEntries_t _steamLeaderboardEntries;
        private CallResult<SteamLeaderboardData> _findResult = new();
        private CallResult<LeaderboardScoresDownloaded_t> _entriesResult  = new();
        #endregion
        #region Events
        public event Action<SteamLeaderboardEntries_t> LeaderboardTaken;
        #endregion
        #region Overrides
        protected override void OnAfterAwake()
        {
            Initialize();
        }
        #endregion
        #region Behaviour Callbacks
        private void OnLeaderBoardFindResult(SteamLeaderboardData result, bool failure)
        {
            if (result.LeaderBoardResult.m_bLeaderboardFound == 0)
                return;
            Debug.Log("Steam leaderboards: Found - " + result.LeaderBoardResult.m_bLeaderboardFound + " leaderboardID - " + result.LeaderBoardResult.m_hSteamLeaderboard.m_SteamLeaderboard);
            _dictLeaderboard.Add(result.LeaderboardName, result.LeaderBoardResult.m_hSteamLeaderboard);
        }
        private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
        {
            Debug.Log("[" + LeaderboardScoresDownloaded_t.k_iCallback + " - LeaderboardScoresDownloaded] - " + pCallback.m_hSteamLeaderboard + " -- " + pCallback.m_hSteamLeaderboardEntries + " -- " + pCallback.m_cEntryCount);
            _steamLeaderboardEntries = pCallback.m_hSteamLeaderboardEntries;
            LeaderboardTaken?.Invoke(_steamLeaderboardEntries);
        }
        #endregion
        #region Public Methods
        public void SetLeaderboardEntry(SteamLeaderboardSO leaderboard, int score)
        {
            if(!leaderboard)
            {
                Debug.LogWarning("No leaderboard entry was given.\n", this);
                return;
            }
            if (_dictLeaderboard.ContainsKey(leaderboard.Id))
            {
                SteamUserStats.UploadLeaderboardScore(_dictLeaderboard.GetValueOrDefault(leaderboard.Id), ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);
            }

        }
        public void GetLeaderboardEntries(SteamLeaderboardSO leaderboard)
        {
            if (_dictLeaderboard.ContainsKey(leaderboard.Id))
            {
                SteamAPICall_t leaderboardEntries = SteamUserStats.DownloadLeaderboardEntries(_dictLeaderboard.GetValueOrDefault(leaderboard.Id), ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -10, 10);
                _entriesResult.Set(leaderboardEntries, OnLeaderboardScoresDownloaded);
            }

        }    
        #endregion

        #region Private Methods1
        private void Initialize()
        {
            if(m_leaderboards.Count == 0)
            {
                Debug.LogWarning("There are no leaderboards set to be initialized.\n", this); 
                return;
            }
            foreach (var item in m_leaderboards)
            {
                SteamAPICall_t hSteamAPICall = SteamUserStats.FindLeaderboard(item.Id);
                _findResult.Set(hSteamAPICall, OnLeaderBoardFindResult);
            }
        }
        #endregion

        #region Virtual Methods
        #endregion
    }
}