using Code.Player;
using System.Collections;
using UnityEngine;
using Code.Weapons;
using Code.Graphics;

namespace Code.EnemySystem
{
    public class EnemyBehavior : MonoBehaviour, IDamageable
    {
        public EnemySettings enemySettings;

        private Transform playerPos;
        private PlayerHealth playerHealth;
        private Vector3 wanderDirection;
        private MaskAnimator maskAnimator;
        private WaveSpawner waveSpawner;

        private float elapsedTime = 0f;
        private float remHP;

        private bool reverseDirection = false;
        private bool isChasing = false;

        void Start()
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
            waveSpawner = GameObject.FindFirstObjectByType<WaveSpawner>();
            maskAnimator = GetComponent<MaskAnimator>();

            remHP = enemySettings.HP;
            switch (enemySettings.DamageType)
            {
                case DamageType.Red:
                    maskAnimator.SetColorType(0);
                    break;

                case DamageType.Green:
                    maskAnimator.SetColorType(1);
                    break;

                case DamageType.Blue:
                    maskAnimator.SetColorType(2);
                    break;

                case DamageType.Gold:
                    maskAnimator.SetColorType(3);
                    break;
            }
            SetRandomWanderDirection();
        }

        void Update()
        {
            if (playerPos != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerPos.position);

                if (isChasing)
                {
                    elapsedTime += Time.deltaTime;

                    if (elapsedTime >= enemySettings.chaseTime || distanceToPlayer > enemySettings.visionRange || distanceToPlayer > enemySettings.maxDistanceFromPlayer)
                    {
                        isChasing = false;
                        elapsedTime = 0f;
                        SetRandomWanderDirection();
                    }
                    else
                    {
                        ChasePlayer(distanceToPlayer);
                    }
                }
                else
                {
                    Wander();

                    if (distanceToPlayer <= enemySettings.visionRange)
                    {
                        isChasing = true;
                        elapsedTime = 0f;
                    }
                }
            }
        }

        void Wander()
        {
            float distanceToDestination = Vector3.Distance(transform.position, transform.position + wanderDirection);

            if (distanceToDestination < 0.5f)
            {
                if (Vector3.Distance(transform.position, playerPos.position) > enemySettings.maxDistanceFromPlayer)
                {
                    SetWanderDirectionTowardsPlayer();
                }
                else
                {
                    SetRandomWanderDirection();
                }
            }

            float speed = reverseDirection ? -enemySettings.wanderSpeed : enemySettings.wanderSpeed;

            // Calcola la rotazione solo sull'asse Y
            if (wanderDirection != Vector3.zero)
            {
                float targetAngleY = Mathf.Atan2(wanderDirection.x, wanderDirection.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * enemySettings.rotationSpeed);
            }

            // Applica il movimento avanti
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }



        void SetRandomWanderDirection()
        {
            if (waveSpawner.spawnPoints.Count > 0)
            {
                Transform randomSpawnPoint = waveSpawner.spawnPoints[Random.Range(0, waveSpawner.spawnPoints.Count)];
                wanderDirection = (randomSpawnPoint.position - transform.position).normalized;
            }
            else
            {
                Debug.LogWarning("No spawn points assigned to the enemy.");
                wanderDirection = Vector3.zero; // fallback to avoid unexpected behavior
            }
        }

        void SetWanderDirectionTowardsPlayer()
        {
            wanderDirection = (playerPos.position - transform.position).normalized;
        }

        void ChasePlayer(float _distanceToPlayer)
        {
     
            wanderDirection = (playerPos.position - transform.position).normalized;

            float targetAngleY = Mathf.Atan2(wanderDirection.x, wanderDirection.z) * Mathf.Rad2Deg;


            Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);


            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * enemySettings.rotationSpeed);


            float horizontalDistance = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(playerPos.position.x, 0f, playerPos.position.z));


            if (horizontalDistance > enemySettings.attackRange)
            {
                float speed = enemySettings.chaseSpeed;
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else
            {
                AttackPlayer();
            }
        }





        private void AttackPlayer()
        {
            maskAnimator.AnimateLaughter();
            playerHealth.GetDamage(enemySettings.damage);
            Debug.Log("HAHAH");
        }

        private void Dead()
        {
            Destroy(gameObject);
        }

        public bool GetDamage(DamageType damageType)
        {
            return damageType.HasFlag(enemySettings.DamageType);
        }

        public void ApplyDamage(float amount)
        {
            remHP -= amount;

            if (remHP <= 0)
            {
                Dead();
            }
        }
    }
}
