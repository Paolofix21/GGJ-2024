using Audio;
using Code.EnemySystem.Wakakas;
using Code.Weapons;
using UnityEngine;

namespace Enemies.BossRoberto {
    [RequireComponent(typeof(Rigidbody))]
    public class CameraBullet : MonoBehaviour {
        #region Public Variables
        [SerializeField] private float m_damage = 1;
        [SerializeField] private float m_lifeTime = 5;
        [SerializeField] private DamageType m_damageType = DamageType.All;

        [Space]
        [SerializeField] private SoundSO m_impactSound;
        #endregion

        #region Private Variables
        private Rigidbody _body;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _body = GetComponent<Rigidbody>();
            Destroy(gameObject, m_lifeTime);
        }

        private void OnCollisionEnter(Collision other) {
            var health = other.gameObject.GetComponentInParent<WakakaHealth>();
            if (health && health.GetDamage(m_damageType))
                health.ApplyDamage(m_damage, gameObject);

            AudioManager.Singleton.PlayOneShotWorld(m_impactSound.GetSound(), transform.position, MixerType.SoundFx);
            Destroy(gameObject);
        }
        #endregion

        #region Public Methods
        public void OverrideDamage(float damage) => m_damage = damage;

        public void Launch(Vector3 velocity) {
            _body.velocity = velocity;
            transform.forward = velocity;
        }
        #endregion
    }
}