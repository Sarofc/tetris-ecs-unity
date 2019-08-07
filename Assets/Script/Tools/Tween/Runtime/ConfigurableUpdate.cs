using System;
using UnityEngine;

namespace Saro {
    public abstract class ConfigurableUpdate : MonoBehaviour {
        [SerializeField]
        private UpdateMode m_UpdateMode = UpdateMode.Update;

        private bool m_Registered = false;

        public UpdateMode UpdateMode {
            get { return m_UpdateMode; }
            set {
                if (m_UpdateMode != value) {
                    if (m_Registered) {
                        GameEntry.UnregisterUpdateEvent (m_UpdateMode, OnUpdate);
                        m_UpdateMode = value;
                        GameEntry.RegisterUpdateEvent (m_UpdateMode, OnUpdate);

#if UNITY_EDITOR
                        m_AddUpdateMode = m_UpdateMode;
#endif

                    } else {
                        m_UpdateMode = value;
                    }
                }
            }
        }

        protected virtual void OnEnable () {
            GameEntry.RegisterUpdateEvent (m_UpdateMode, OnUpdate);
            m_Registered = true;

#if UNITY_EDITOR
            m_AddUpdateMode = m_UpdateMode;
#endif
        }

        protected virtual void OnDisable () {
            GameEntry.UnregisterUpdateEvent (m_UpdateMode, OnUpdate);
            m_Registered = false;
        }

        protected abstract void OnUpdate ();

#if UNITY_EDITOR
        private UpdateMode m_AddUpdateMode;
        protected virtual void OnValidate () {
            if (m_Registered && m_AddUpdateMode != m_UpdateMode) {
                GameEntry.UnregisterUpdateEvent (m_AddUpdateMode, OnUpdate);
                GameEntry.RegisterUpdateEvent (m_AddUpdateMode = m_UpdateMode, OnUpdate);
            }
        }
#endif
    }
}