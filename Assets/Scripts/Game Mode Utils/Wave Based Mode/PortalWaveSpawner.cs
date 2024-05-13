using System.Collections;
using UnityEngine;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(SpawnPoint))]
    public class PortalWaveSpawner : MonoBehaviour {
        #region Public Variables
        [SerializeField] private MinorWaveInfoSO m_wave;
        #endregion

        #region Private Variables
        private SpawnPoint _spawnPoint;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => _spawnPoint = GetComponent<SpawnPoint>();

        private IEnumerator Start() {
            m_wave.Init();

            _spawnPoint.AnimatePortal(true);
            yield return new WaitForSeconds(1f);

            var wait = new WaitForSeconds(m_wave.SpawnDelay);
            while (m_wave.TryExtraction(out var entity)) {
                Instantiate(entity, transform.position, Quaternion.Euler(90, 0, 0));
                yield return wait;
            }

            _spawnPoint.AnimatePortal(false);
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
        #endregion
    }
}