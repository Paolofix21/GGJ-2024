using System.Collections;
using System.Collections.Generic;
using Code.GameModeUtils.WaveBasedMode;
using Code.Promises;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Core.MatchManagers {
    [DefaultExecutionOrder(-1)]
    public sealed class WaveBasedEntityManager : EntityManager {
        #region Public Variables
        [SerializeField] private WavesCollectionSO m_wavesDataPack;

        public event System.Action OnFinish;
        public event System.Action<int> OnWaveChanged;
        #endregion

        #region Private Variables
        private readonly List<SpawnPoint> _pointsOfInterest = new();
        private readonly List<SpawnPoint> _pointsOfInterestExtracted = new();

        private MajorWaveInfo _currentWave;
        #endregion

        #region Behaviour Callbacks
        protected override void OnAfterAwake() {
            _pointsOfInterest.AddRange(FindObjectsByType<SpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
            GameEvents.OnCutsceneStateChanged += ResumeWavesAfterCutscene;
        }

        private void Start() => WaveBasedMatchManager.Singleton.SetEntityManager(this);

        protected override void OnBeforeDestroy() {
            GameEvents.OnCutsceneStateChanged -= ResumeWavesAfterCutscene;
        }
        #endregion

        #region Overrides
        public override void Begin() {
            m_wavesDataPack.Init();

            if (m_wavesDataPack.TryGetNextWave(out _currentWave))
                SpawnNextWave();
        }

        public override void Enable() {
            Entities.ForeEach(e => e.Enable());
        }

        public override void Disable() {
            Entities.ForeEach(e => e.Disable());
        }

        public override void End() {
            
        }

        protected override void OnEntitiesCleared() => SpawnNextWave();
        #endregion

        #region Public Methods
        public void SpawnWaveCustom(MinorWaveInfoSO wave) {
            wave.Init();
            SpawnAllEntities(wave, 0f);
        }
        #endregion

        #region Private Methods
        private void SpawnNextWave() {
            if (_currentWave.TryGetNextSubWave(out var minorWaveInfo)) {
                SpawnAllEntities(minorWaveInfo, _currentWave.Delay);
                return;
            }

            // No minor wave has been extracted, the current wave is over
            if (m_wavesDataPack.TryGetNextWave(out _currentWave)) {
                // TODO - Should wait for confirmation before starting next wave?
                // SpawnNextWave();
                OnWaveChanged?.Invoke(m_wavesDataPack.GetIndex());
                return;
            }

            // No major wave has been extracted, waves are over
            OnFinish?.Invoke();
        }

        private void SpawnEntity(IEntity entity) {
            if (_pointsOfInterest.Count <= 0) {
                _pointsOfInterest.AddRange(_pointsOfInterestExtracted);
                _pointsOfInterestExtracted.Clear();
            }

            var index = Random.Range(0, _pointsOfInterest.Count);
            var poi = _pointsOfInterest[index];

            var entityInstance = (IEntity)Instantiate(entity as MonoBehaviour, poi.Position, poi.Rotation);
            AddEntity(entityInstance);

            if (GameEvents.IsOnHold)
                entityInstance.Disable();

            _pointsOfInterest.RemoveAt(index);
            _pointsOfInterestExtracted.Add(poi);
        }

        private void SpawnAllEntities(MinorWaveInfoSO minorWaveInfo, float delay) => StartCoroutine(SpawnAllEntitiesCO(minorWaveInfo, delay));
        private IEnumerator SpawnAllEntitiesCO(MinorWaveInfoSO minorWaveInfo, float delay) {
            yield return new WaitForSeconds(delay);

            var spawnDelay = new WaitForSeconds(minorWaveInfo.SpawnDelay);

            while (minorWaveInfo.TryExtraction(out var entity)) {
                if (GameEvents.IsOnHold)
                    yield return null;

                SpawnEntity(entity);
                yield return spawnDelay;
            }
        }
        #endregion

        #region Event Methods
        private void ResumeWavesAfterCutscene(bool cutscenePlaying) {
            if (cutscenePlaying)
                return;

            SpawnNextWave();
        }
        #endregion
    }
}