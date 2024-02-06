﻿using Code.Core.MatchManagers;
using Code.EnemySystem.Boss;
using Code.Promises;
using UnityEngine;
using Utilities;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBossBehaviour))]
    public class WaveBasedBossEntity : MonoBehaviour, IEntity {
        #region Public Variables
        public event System.Action OnFinish;
        #endregion

        #region Private Variables
        private WakakaBossBehaviour _controller;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _controller = GetComponent<WakakaBossBehaviour>();
            _controller.OnSurrender += Surrender;

            Disable();
        }

        private void Start() => WaveBasedMatchManager.Singleton.SetBoss(this);

        private void OnDestroy() => OnDestroyed?.Invoke(this);
        #endregion

        #region IEntity
        public event DestroyEventHandler<IEntity> OnDestroyed;

        public Transform Transform => transform;

        public void Enable() {
            _controller.enabled = true;
        }

        public void Disable() {
            _controller.enabled = false;
        }
        #endregion

        #region Public Methods
        public void StartFight() => _controller.BeginFight();
        #endregion

        #region Event Methods
        private void Surrender() => OnFinish?.Invoke();
        #endregion
    }
}