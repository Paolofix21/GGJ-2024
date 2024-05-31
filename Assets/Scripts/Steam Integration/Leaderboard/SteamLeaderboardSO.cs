using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SteamIntegration.Leaderboard
{
    [CreateAssetMenu(menuName = "Steam/Leaderboard", fileName = "New leaderboard")]
    public sealed class SteamLeaderboardSO : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
    }
}