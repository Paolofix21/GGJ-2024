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
        [SerializeField] private ParticleSystem particle = default;

        private Transform playerPos;
        private PlayerHealth playerHealth;
        private Vector3 wanderDirection;
        private MaskAnimator maskAnimator;
        private WaveSpawner waveSpawner;

        private float elapsedTime = 0f;
        private float remHP;

        private float attackCooldown = 2f; // Tempo di attesa tra gli attacchi
        private float currentCooldown = 0f;

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
            StartCoroutine(CheckDistanceToOriginRoutine());
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

            // Se sei abbastanza vicino al punto di destinazione, calcola una nuova direzione casuale
            if (distanceToDestination < 1f)
            {
                SetRandomWanderDirection();
            }

            float speed = reverseDirection ? -enemySettings.wanderSpeed : enemySettings.wanderSpeed;

            
            if (wanderDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(wanderDirection);
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


        void ChasePlayer(float _distanceToPlayer)
        {
            wanderDirection = (playerPos.position - transform.position).normalized;

            /*float targetAngleY = Mathf.Atan2(wanderDirection.x, wanderDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * enemySettings.rotationSpeed);*/

            float horizontalDistance = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(playerPos.position.x, 0f, playerPos.position.z));

            transform.rotation = Quaternion.LookRotation(wanderDirection);

            if (currentCooldown <= 0f)
            {
                if (horizontalDistance > enemySettings.attackRange)
                {
                    float speed = enemySettings.chaseSpeed;
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                }
                else
                {
                    AttackPlayer();
                    currentCooldown = attackCooldown; 
                }
            }
            else
            {
                currentCooldown -= Time.deltaTime; 
            }
        }

        IEnumerator CheckDistanceToOriginRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(5f);

                float distanceToOrigin = Vector3.Distance(transform.position, Vector3.zero);

                if (distanceToOrigin > 20f)
                {
                    Vector3 targetPosition = playerPos.position;
                    transform.Translate((targetPosition - transform.position).normalized * enemySettings.wanderSpeed * Time.deltaTime);
                }
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
            Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        public bool GetDamage(DamageType damageType)
        {
            Debug.Log($"Asking if can be damaged...\nType: {damageType}\n", this);
            return damageType.HasFlag(enemySettings.DamageType);
        }

        public void ApplyDamage(float amount)
        {
            Debug.Log($"Applying {amount} damage...\n", this);
            remHP -= amount;

            if (remHP <= 0)
            {
                Dead();
            }
        }
    }
}
