using Code.Player;
using System.Collections;
using UnityEngine;

namespace Code.EnemySystem
{
    public class EnemyBehavior : MonoBehaviour
    {
        public EnemySettings enemySettings;

        private Transform playerPos;
        private PlayerHealth playerHealth;
        private float elapsedTime = 0f;
        private bool isChasing = false;

        private Vector3 wanderDirection;
        private bool reverseDirection = false;

        void Start()
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
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
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= enemySettings.wanderTime)
            {
                if (Vector3.Distance(transform.position, playerPos.position) > enemySettings.maxDistanceFromPlayer)
                {
                    SetWanderDirectionTowardsPlayer();
                }
                else
                {
                    SetRandomWanderDirection();
                }

                elapsedTime = 0f;
            }

            float speed = reverseDirection ? -enemySettings.wanderSpeed : enemySettings.wanderSpeed;
            transform.Translate(wanderDirection * speed * Time.deltaTime);
        }

        void SetRandomWanderDirection()
        {
            float randomAngleX = Random.Range(-180f, 180f);
            float randomAngleY = Random.Range(-180f, 180f);

            wanderDirection = Quaternion.Euler(randomAngleX, randomAngleY, 0f) * transform.forward;
        }

        void SetWanderDirectionTowardsPlayer()
        {
            wanderDirection = (playerPos.position - transform.position).normalized;
        }

        void ChasePlayer(float _distanceToPlayer)
        {
            Vector3 directionToPlayer = (playerPos.position - transform.position).normalized;
            transform.Translate(directionToPlayer * enemySettings.chaseSpeed * Time.deltaTime);
            if (_distanceToPlayer <= enemySettings.attackRange)
            {
                AttackPlayer();
            }
        }


        private void AttackPlayer() // valutare se fare un delay
        {
            playerHealth.GetDamage(enemySettings.damage);
            Debug.Log("HAHAHHA");
        }

    }
}
