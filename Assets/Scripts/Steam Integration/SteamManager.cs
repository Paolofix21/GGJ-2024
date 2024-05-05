﻿using Code.Utilities;
using SteamIntegration.Account;
using SteamIntegration.Achievements;
using SteamIntegration.Statistics;
using Steamworks;
using UnityEngine;

namespace SteamIntegration {
    public class SteamManager : SingletonBehaviour<SteamManager> {
        #region Constants
        private const string k_achievementsControllerResourcePath = "Managers/# Steam Achievements Controller";
        private const string k_statisticsControllerResourcePath = "Managers/# Steam Statistics Controller";
        #endregion

        #region Properties
        public static bool IsInitialized { get; private set; }

        public SUser User { get; private set; }
        #endregion

        #region Constructors
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize() {
            if (!SteamAPI.IsSteamRunning()) {
                Debug.LogWarning("[Steamworks.NET] SteamNotRunningException\nSteam App must be running for the Steam API to function\n");
                return;
            }

            if (!SteamAPI.Init()) {
                Debug.LogWarning("[Steamworks.NET] SteamInitFailedException\nAn error prevented Steam API from initializing\n");
                return;
            }

            var obj = new GameObject("# Steam Manager");
            obj.AddComponent<SteamManager>().Persist();
        }
        #endregion

        #region Behaviour Callbacks
        protected override void OnAfterAwake() {
            User = new SUser {
                Id = SteamUser.GetSteamID().m_SteamID,
                Name = SteamFriends.GetPersonaName()
            };

            var appOwner = SteamApps.GetAppOwner();

            Debug.Log($"User  {User.Id}\nOwner {appOwner.m_SteamID}\n");

            Instantiate(Resources.Load<SteamStatisticsController>(k_statisticsControllerResourcePath));
            Instantiate(Resources.Load<SteamAchievementsController>(k_achievementsControllerResourcePath));
        }

        private void Update() {
            if (!IsInitialized)
                return;

            SteamAPI.RunCallbacks();
        }

        protected override void OnBeforeDestroy() {
            if (!IsInitialized)
                return;

            SteamAPI.Shutdown();
        }
        #endregion
    }
}