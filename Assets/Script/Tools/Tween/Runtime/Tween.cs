using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Saro {
    public enum TimeMode {
        Normal,
        Unscaled
    }

    public enum WrapMode {
        Clamp,
        Loop,
        PingPong
    }

    public enum ArrivedAction {
        KeepPlaying = 0,
        StopOnForwardArrived,
        StopOnBackArrived,
        AlwaysStopOnArrived
    }

    public enum PlayDirection {
        Forward,
        Backward
    }

    [DisallowMultipleComponent]
    public partial class Tween : ConfigurableUpdate {
        // private const float c_MinDuration = 0.0001f;

        [SerializeField]
        private float m_Duration = 1f;

        [SerializeField] private TimeMode m_TimeMode = TimeMode.Unscaled;
        [SerializeField] private WrapMode m_WrapMode = WrapMode.Clamp;

        [SerializeField] private ArrivedAction m_ArrivedAction = ArrivedAction.AlwaysStopOnArrived;

        [SerializeField] private UnityEvent m_OnForwardArrived;
        [SerializeField] private UnityEvent m_OnBackwardArrived;

        [SerializeField] private List<TweenAnimation> m_Animations;

        private float m_NormalizedTime = 0f;

        public PlayDirection playDirection;
        public float Duration {
            get { return m_Duration; }
            set { m_Duration = value; } //> c_MinDuration?value : c_MinDuration;}
        }

        public event UnityAction OnForwardArrived {
            add {
                if (m_OnForwardArrived == null) m_OnForwardArrived = new UnityEvent ();
                m_OnForwardArrived.AddListener (value);
            }
            remove {
                m_OnForwardArrived?.RemoveListener (value);
            }
        }
        public event UnityAction OnBackwardArrived {
            add {
                if (m_OnBackwardArrived == null) m_OnBackwardArrived = new UnityEvent ();
                m_OnBackwardArrived.AddListener (value);
            }
            remove {
                m_OnBackwardArrived?.RemoveListener (value);
            }
        }

        public float NormalizedTime {
            get { return m_NormalizedTime; }
            set {
                m_NormalizedTime = Mathf.Clamp01 (value);
                if (m_Animations != null) {
                    for (int i = 0; i < m_Animations.Count; i++) {
                        var anim = m_Animations[i];
                        if (anim) {
                            if (anim.enabled) anim.OnUpdate (m_NormalizedTime);
                        }
                    }
                }
            }
        }

        public void ResetNormalizedTime()
        {
            m_NormalizedTime = 0f;
        }

        public void ReverseDirection () {
            playDirection = playDirection == PlayDirection.Forward ? PlayDirection.Backward : PlayDirection.Forward;
        }

        public void SetDirectionForward () {
            playDirection = PlayDirection.Forward;
        }

        public void SetDirectionBackward () {
            playDirection = PlayDirection.Backward;
        }

        public void ForwardEnable () {
            enabled = true;
            SetDirectionForward ();
        }

        public void BackwardEnable () {
            enabled = true;
            SetDirectionBackward ();
        }


        /// <summary>
        /// Add TweenAnimation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddAnimation<T> () where T : TweenAnimation {
            var anim = gameObject.AddComponent<T> ();
            AddAnimationInternal (anim);
            return anim;
        }

        public bool RemoveAnimation<T> (T anim) where T : TweenAnimation {

            if (m_Animations.Remove (anim)) {
                if (Application.isPlaying) Destroy (anim);
                else DestroyImmediate (anim);
                return true;
            }
            return false;
        }

        private void AddAnimationInternal<T> (T anim) where T : TweenAnimation {
            anim.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            if (m_Animations == null) m_Animations = new List<TweenAnimation> ();
            m_Animations.Add (anim);
        }

        protected override void OnUpdate () {
            if (m_Duration == 0) {
                return;
            }

#if UNITY_EDITOR
            if (m_Dragging) return;
#endif

            float deltaTime =
#if UNITY_EDITOR
                !Application.isPlaying ? GameEntry.DeltaTimeInEditor :
#endif
                (m_TimeMode == TimeMode.Normal ? Time.deltaTime : Time.unscaledDeltaTime);

            while (enabled && deltaTime > Mathf.Epsilon) {
                if (playDirection == PlayDirection.Forward) {
                    if (m_WrapMode == WrapMode.Clamp && NormalizedTime == 1f) {
                        NormalizedTime = 1f;
                        return;
                    }

                    float time = NormalizedTime * m_Duration + deltaTime;

                    if (time < m_Duration) {
                        NormalizedTime = time / m_Duration;
                        return;
                    }

                    deltaTime = time - m_Duration;
                    NormalizedTime = 1f;

                    if (m_WrapMode == WrapMode.PingPong) playDirection = PlayDirection.Backward;
                    else if (m_WrapMode == WrapMode.Loop) NormalizedTime = 0f;

                    if ((m_ArrivedAction & ArrivedAction.StopOnForwardArrived) == ArrivedAction.StopOnForwardArrived)
                        enabled = false;

                    m_OnForwardArrived?.Invoke ();
                } else {
                    if (m_WrapMode == WrapMode.Clamp && NormalizedTime == 0f) {
                        NormalizedTime = 0f;
                        return;
                    }

                    float time = NormalizedTime * m_Duration - deltaTime;
                    if (time > 0f) {
                        NormalizedTime = time / m_Duration;
                        return;
                    }

                    deltaTime = -time;
                    NormalizedTime = 0f;

                    if (m_WrapMode == WrapMode.PingPong) playDirection = PlayDirection.Forward;
                    else if (m_WrapMode == WrapMode.Loop) NormalizedTime = 1f;

                    if ((m_ArrivedAction & ArrivedAction.StopOnBackArrived) == ArrivedAction.StopOnBackArrived)
                        enabled = false;

                    m_OnBackwardArrived?.Invoke ();
                }

            }
        }

        private void OnDestroy () {
            if (m_Animations != null) {
                for (int i = 0; i < m_Animations.Count; i++) {
                    if (m_Animations[i]) {
#if UNITY_EDITOR
                        if (!Application.isPlaying)
                            m_ObjectList.Add (gameObject);
                        else
#endif
                            Destroy (m_Animations[i]);
                    }
                }
            }
        }

    }
}