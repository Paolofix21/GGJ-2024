using UnityEngine;

namespace BaseEnemy
{
    public class EnemyBehavior : MonoBehaviour
    {
        public float wanderSpeed = 2f;
        public float chaseSpeed = 5f;
        public float visionRange = 10f;
        public float wanderTime = 3f;
        public float chaseTime = 5f;

        private Transform player;
        private float elapsedTime = 0f;
        private bool isChasing = false;
        private Vector3 wanderDirection;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            SetRandomWanderDirection();
        }

        void Update()
        {
            if (player != null)
            {
                if (IsPlayerInVisionRange())
                {
                    elapsedTime += Time.deltaTime;

                    if (isChasing)
                    {
                        ChasePlayer();

                        if (elapsedTime >= chaseTime)
                        {
                            isChasing = false;
                            elapsedTime = 0f;
                        }
                    }
                    else
                    {
                        isChasing = true;
                        elapsedTime = 0f;
                    }
                }
                else
                {
                    Wander();
                }
            }
        }

        bool IsPlayerInVisionRange()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                return distanceToPlayer <= visionRange;
            }

            return false;
        }

        void Wander()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= wanderTime)
            {
                SetRandomWanderDirection();
                elapsedTime = 0f;
            }

            Vector3 horizontalMovement = new Vector3(wanderDirection.x, 0f, wanderDirection.y);
            transform.Translate(horizontalMovement * wanderSpeed * Time.deltaTime);
        }

        void SetRandomWanderDirection()
        {
            wanderDirection = Random.insideUnitCircle.normalized;
        }

        void ChasePlayer()
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 horizontalDirectionToPlayer = new Vector3(directionToPlayer.x, 0f, directionToPlayer.y);
            transform.Translate(horizontalDirectionToPlayer * chaseSpeed * Time.deltaTime);
        }
    }
}
