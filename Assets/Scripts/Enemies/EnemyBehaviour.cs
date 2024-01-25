using UnityEngine;

namespace Code.EnemySystem
{
    public class EnemyBehavior : MonoBehaviour
    {
        public EnemySettings enemySettings;

        private Transform player;
        private float elapsedTime = 0f;
        private bool isChasing = false;

        private Vector3 wanderDirection;
        private bool reverseDirection = false;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            SetRandomWanderDirection();
        }

        void Update()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
                        ChasePlayer();
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
                if (Vector3.Distance(transform.position, player.position) > enemySettings.maxDistanceFromPlayer)
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
            wanderDirection = (player.position - transform.position).normalized;
        }

        void ChasePlayer()
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            transform.Translate(directionToPlayer * enemySettings.chaseSpeed * Time.deltaTime);
        }

        void AttackPlayer()
        {

        }

    }
}
