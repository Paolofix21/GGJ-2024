using Code.Weapons;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "ScriptableObjects/Enemy Settings", order = 1)]
public class EnemySettings : ScriptableObject
{
    [Header("Statistics Settings")]
    public float HP = 50f;
    public float damage = 5f;
    public DamageType DamageType;

    [Header("Movement Settings")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 5f;
    public float maxDistanceFromPlayer = 20f;
    public float rotationSpeed = 1.0f;

    [Header("Detection Settings")]
    public float visionRange = 10f;
    public float attackRange = 2f;

    [Header("Wander Settings")]
    public float wanderTime = 3f;
    public float chaseTime = 5f;
}
