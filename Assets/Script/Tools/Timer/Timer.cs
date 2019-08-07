using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saro {

    public static class TimerExtention {

        public static Timer Register (this MonoBehaviour behaviour, float duration, Action onComplete, Action onUpdate = null,
            bool isLooped = false, bool useRealTime = false) {

            return Timer.Register (duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
        }
    }

    public class Timer {

        public float duration { get; set; }
        public bool isLooped { get; set; }
        public bool isCompleted { get; private set; }
        public bool dispose { get; set; }
        public bool useRealTime { get; private set; }
        public bool isPaused {
            get { return m_TimeElapsedBeforePause.HasValue; }
        }
        public bool isCancelled {
            get { return m_TimeElapsedBeforeCancel.HasValue; }
        }

        public bool isDone {
            get { return  (isCompleted || isCancelled || isOwnerDestroyed); }
        }

        // TimerMgr Instance
        private static TimerManager TimerMgrInstance;

        private bool isOwnerDestroyed {
            get { return m_HasAutoDestroyOwner && m_AutoDestroyOwner == null; }
        }

        // complete callback
        private Action m_OnComplete;
        // update callback
        private Action m_OnUpdate;
        private float? m_StartTime;
        private float m_LastUpdateTime;

        private float? m_TimeElapsedBeforeCancel;
        private float? m_TimeElapsedBeforePause;

        private MonoBehaviour m_AutoDestroyOwner;
        private bool m_HasAutoDestroyOwner;

        private Timer (float duration, Action onComplete, Action onUpdate, bool dispose,
            bool isLooped, bool useRealTime, MonoBehaviour autoDestroyerOwner) {

            this.duration = duration;
            this.isLooped = isLooped;
            this.useRealTime = useRealTime;

            this.m_OnComplete = onComplete;
            this.m_OnUpdate = onUpdate;
            this.dispose = dispose;

            this.m_AutoDestroyOwner = autoDestroyerOwner;
            this.m_HasAutoDestroyOwner = autoDestroyerOwner != null;
        }

        /// <summary>
        /// 注册一个Timer。
        ///
        /// warning ：如果需要对Timer的回调函数进行Cache，需要将dispose设为false
        /// </summary>
        /// <param name="duration">持续时间</param>
        /// <param name="onComplete">Timer完成后调用</param>
        /// <param name="onUpdate">Timer执行时调用</param>
        /// <param name="dispose">当Timer执行完毕时，清除委托</param>
        /// <param name="isLooped"></param>
        /// <param name="useRealTime"></param>
        /// <param name="DestroyerOwner"></param>
        /// <returns></returns>
        public static Timer Register (float duration, Action onComplete = null, Action onUpdate = null, bool dispose = true,
            bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyerOwner = null) {

            // Lazy Initialize.
            // Initialize TimerMagr Instance.
            if (TimerMgrInstance == null) {
                // TimerMgrInstance = (new GameObject ("_TimerManager")).AddComponent<TimerManager> ();
                TimerMgrInstance = new TimerManager ();
                GameEntry.OnUpdate += TimerMgrInstance.OnUpdate;
            }

            if (TimerMgrInstance == null) Debug.LogError ("NULL TimerMgrInstance");

            // create new Timer
            Timer timer = new Timer (duration, onComplete, onUpdate, dispose, isLooped, useRealTime, autoDestroyerOwner);

            return timer;
        }

        public void Start () {
            this.m_StartTime = GetWorldTime ();
            this.m_LastUpdateTime = this.m_StartTime.Value;
            // add to TimerMgr
            TimerMgrInstance.RegisterTimer (this);
        }

        public void Restart () {
            // reset
            this.isCompleted = false;
            this.m_TimeElapsedBeforeCancel = null;
            this.m_TimeElapsedBeforePause = null;

            Start ();
        }

        public static void Cancel (Timer timer) {
            if (timer != null) {
                timer.Cancel ();
            }
        }

        public static void Pause (Timer timer) {
            if (timer != null) {
                timer.Pause ();
            }
        }

        public static void Resume (Timer timer) {
            if (timer != null) {
                timer.Resume ();
            }
        }

        public static void CancelAllRegisteredTimers () {
            if (TimerMgrInstance != null) {
                TimerMgrInstance.CancelAllTimers ();
            }
        }

        public static void PauseAllRegisteredTimers () {
            if (TimerMgrInstance != null) {
                TimerMgrInstance.PauseAllTimers ();
            }
        }

        public static void ResumeAllRegisteredTimers () {
            if (TimerMgrInstance != null) {
                TimerMgrInstance.ResumeAllTimers ();
            }
        }

        #region public methods

        public void Cancel () {
            if (isDone) {
                return;
            }
            m_TimeElapsedBeforeCancel = GetTimeElapsed ();
            m_TimeElapsedBeforePause = null;
        }

        public void Pause () {
            if (isPaused || isDone) { return; }

            m_TimeElapsedBeforePause = GetTimeElapsed ();
        }

        public void Resume () {
            if (!isPaused || isDone) { return; }

            m_TimeElapsedBeforePause = null;
        }

        public float GetTimeElapsed () {
            if (isCompleted || GetWorldTime () >= GetFireTime ()) {
                return duration;
            }

            if (!m_StartTime.HasValue) return 0;

            // float？ 可空类型修饰符
            // ??      合并空 运算符
            // if m_TimeElapsedBeforeCancel is not null, return m_TimeElapsedBeforeCancel.
            return m_TimeElapsedBeforeCancel ??
                m_TimeElapsedBeforePause ??
                GetWorldTime () - m_StartTime.Value;
        }

        public void ClearEvent () {
            m_OnComplete = null;
            m_OnUpdate = null;
        }

        #endregion

        private float GetWorldTime () {
            return useRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime () {
            return m_StartTime.Value + duration;
        }

        private void Update () {
            // 确然timer是否结束
            if (isDone) {
                return;
            }
            // 确认timer是否暂停
            if (isPaused) {
                m_StartTime += GetWorldTime () - m_LastUpdateTime;
                m_LastUpdateTime = GetWorldTime ();
                return;
            }
            // 记录上一帧的时间
            m_LastUpdateTime = GetWorldTime ();

            m_OnUpdate?.Invoke ( /* GetTimeElapsed () */ );
            // 确认timer是否完成
            if (GetWorldTime () >= GetFireTime ()) {

                m_OnComplete?.Invoke ();

                if (isLooped) {
                    m_StartTime = GetWorldTime ();
                } else {
                    isCompleted = true;
                    m_StartTime = null;

                    if (dispose) {
                        ClearEvent ();
                    }
                }
            }
        }

        private class TimerManager {

            private List<Timer> m_timers = new List<Timer> ();

            public void RegisterTimer (Timer timer) {
                m_timers.Add (timer);
            }

            public void CancelAllTimers () {
                for (int i = 0; i < m_timers.Count; i++) {
                    m_timers[i].Cancel ();
                }

                m_timers.Clear ();
            }

            public void PauseAllTimers () {
                for (int i = 0; i < m_timers.Count; i++) {
                    m_timers[i].Pause ();
                }
            }

            public void ResumeAllTimers () {
                for (int i = 0; i < m_timers.Count; i++) {
                    m_timers[i].Resume ();
                }
            }

            public void OnUpdate () {
                UpdateAllTimers ();
            }

            private void UpdateAllTimers () {

                for (int i = 0; i < m_timers.Count; i++) {
                    m_timers[i].Update ();
                }

                m_timers.RemoveAll (t => t.isDone);
            }
        }
    }
}