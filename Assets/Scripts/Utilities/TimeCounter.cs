using UnityEngine;

namespace Utilities {
    public sealed class TimeCounter {
        #region Private Variables
        private float _pausedTimeStart;
        private float _pausedTimeUnscaledStart;

        private float _pausedTime;
        private float _pausedTimeUnscaled;
        #endregion

        #region Properties
        public bool HasStarted { get; private set; }
        public bool HasStopped { get; private set; }
        public bool IsRunning { get; private set; }

        public float StartTime { get; private set; }
        public float StartTimeUnscaled { get; private set; }

        public float EndTime { get; private set; }
        public float EndTimeUnscaled { get; private set; }
        #endregion

        #region Public Methods
        public void Start() {
            if (HasStarted)
                return;

            StartTime = Time.time;
            StartTimeUnscaled = Time.unscaledTime;

            IsRunning = true;
            HasStarted = true;
        }

        public void Pause() {
            IsRunning = false;
            _pausedTimeStart = Time.time;
            _pausedTimeUnscaledStart = Time.unscaledTime;
        }

        public void Resume() {
            IsRunning = true;
            _pausedTime += Time.time - _pausedTimeStart;
            _pausedTimeUnscaled += Time.unscaledTime - _pausedTimeUnscaledStart;
        }

        public void End() {
            if (HasStopped)
                return;

            if (!IsRunning) {
                _pausedTime += Time.time - _pausedTimeStart;
                _pausedTimeUnscaled += Time.unscaledTime - _pausedTimeUnscaledStart;
            }

            EndTime = Time.time;
            EndTimeUnscaled = Time.unscaledTime;

            IsRunning = false;
            HasStopped = true;
        }

        public float GetCurrentDuration() => Time.time - StartTime - _pausedTime;
        public float GetCurrentDurationUnscaled() => Time.unscaledTime - StartTimeUnscaled - _pausedTimeUnscaled;

        public float GetTotalDuration() => HasStopped ? EndTime - StartTime - _pausedTime : -1;
        public float GetTotalDurationUnscaled() => HasStopped ? EndTimeUnscaled - StartTimeUnscaled - _pausedTimeUnscaled : -1;
        #endregion
    }
}