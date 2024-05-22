using System.Collections;
using System.Collections.Generic;
using Code.GameModeUtils.WaveBasedMode;
using Code.Promises;
using SteamIntegration.Achievements;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Core.MatchManagers {
    [DefaultExecutionOrder(-1)]
    public sealed class WaveBasedEntityManager : EntityManager {
        #region Public Variables
        [SerializeField] private WavesCollectionSO m_wavesDataPack;
        [SerializeField] private float m_killTimeThreshold = .1f;

        [Space]
        [SerializeField] private SteamAchievementSO m_oneShotOneKillAchievement;

        public event System.Action OnFinish;
        public event System.Action<int> OnWaveChanged;
        #endregion

        #region Private Variables
        private readonly List<SpawnPoint> _pointsOfInterest = new();
        private readonly List<SpawnPoint> _pointsOfInterestExtracted = new();

        private MajorWaveInfo _currentWave;
        private WavesCollectionSO _wavesCollection;

        private float _lastTimeAdded;
        private int _simultaneousKills;
        private int _originalWaveCount;
        #endregion

        #region Behaviour Callbacks
        protected override void OnAfterAwake() {
            _pointsOfInterest.AddRange(FindObjectsByType<SpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
            GameEvents.OnCutsceneStateChanged += ResumeWavesAfterCutscene;
        }

        private void Start() => WaveBasedMatchManager.Singleton.SetEntityManager(this);

        protected override void OnBeforeDestroy() => GameEvents.OnCutsceneStateChanged -= ResumeWavesAfterCutscene;
        #endregion

        #region Overrides
        public override void Begin() {
            _wavesCollection = Instantiate(m_wavesDataPack);
            _wavesCollection.Init();

            OnWaveChanged?.Invoke(0);
        }

        public override void Enable() {
            Entities.ForeEach(e => e.Enable());
        }

        public override void Disable() {
            Entities.ForeEach(e => e.Disable());
        }

        public override void End() {
            Entities.ForeEach(e => Destroy(e.Transform.gameObject));
            Destroy(_wavesCollection);
        }

        protected override void OnEntityRemoved(IEntity element) {
            if (Time.unscaledTime - _lastTimeAdded > m_killTimeThreshold)
                _simultaneousKills = 0;
            else
                _simultaneousKills++;

            _lastTimeAdded = Time.unscaledTime;

            if (_simultaneousKills != _originalWaveCount)
                return;

            if (_wavesCollection.GetRemainingWavesCount() > 0)
                return;

            if (_currentWave.GetRemainingWavesCount() > 0)
                return;

            SteamAchievementsController.Singleton?.AdvanceAchievement(m_oneShotOneKillAchievement);
            Debug.Log($"Unlocked achievement: {m_oneShotOneKillAchievement?.name}\n");
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
            if (_currentWave == null)
                return;

            if (_currentWave.TryGetNextSubWave(out var minorWaveInfo)) {
                SpawnAllEntities(minorWaveInfo, _currentWave.Delay);
                return;
            }

            // No minor wave has been extracted, the current wave is over
            if (_wavesCollection.TryGetNextWave(out _currentWave)) {
                // TODO - Should wait for confirmation before starting next wave?
                // SpawnNextWave();
                OnWaveChanged?.Invoke(_wavesCollection.GetIndex());
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
            _pointsOfInterest.ForEach(poi => poi.AnimatePortal(true));
            _pointsOfInterestExtracted.ForEach(poi => poi.AnimatePortal(true));

            yield return new WaitForSeconds(delay);

            var spawnDelay = new WaitForSeconds(minorWaveInfo.SpawnDelay);
            _originalWaveCount = minorWaveInfo.Count;

            while (minorWaveInfo.TryExtraction(out var entity)) {
                if (GameEvents.IsOnHold)
                    yield return null;

                SpawnEntity(entity);
                yield return spawnDelay;
            }

            _pointsOfInterest.ForEach(poi => poi.AnimatePortal(false));
            _pointsOfInterestExtracted.ForEach(poi => poi.AnimatePortal(false));
        }
        #endregion

        #region Event Methods
        private void ResumeWavesAfterCutscene(bool cutscenePlaying) {
            if (cutscenePlaying)
                return;

            if (_currentWave != null)
                SpawnNextWave();
            else if (_wavesCollection.TryGetNextWave(out _currentWave))
                SpawnNextWave();
        }
        #endregion
    }
}