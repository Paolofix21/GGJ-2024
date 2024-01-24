using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "ScriptableObjects/Enemy Settings", order = 1)]
public class EnemySettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 5f;
    public float maxDistanceFromPlayer = 20f;

    [Header("Detection Settings")]
    public float visionRange = 10f;

    [Header("Wander Settings")]
    public float wanderTime = 3f;
    public float chaseTime = 5f;
}
