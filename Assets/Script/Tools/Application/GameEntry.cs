using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Saro {

    public sealed class GameEntry {

        public static event Action OnInit {
            add {
                m_OnInit += value;
#if UNITY_EDITOR
                m_GlobalApplication.UpdateInfo (value, 1);
#endif
            }
            remove {
                m_OnInit -= value;
#if UNITY_EDITOR
                m_GlobalApplication.UpdateInfo (value, 2);
#endif
            }
        }

        public static event Action OnUpdate {
            add {
                m_OnUpdate += value;
#if UNITY_EDITOR
                m_GlobalApplication?.UpdateInfo (value, 1);
#endif
            }
            remove {
                m_OnUpdate -= value;
#if UNITY_EDITOR
                m_GlobalApplication?.UpdateInfo (value, 2);
#endif
            }
        }

        public static event Action OnFixedUpdate {
            add {
                m_OnFixedUpdate += value;
#if UNITY_EDITOR
                m_GlobalApplication?.UpdateInfo (value, 1);
#endif
            }
            remove {
                m_OnFixedUpdate -= value;
#if UNITY_EDITOR
                m_GlobalApplication?.UpdateInfo (value, 2);
#endif
            }
        }

        public static event Action OnLateUpdate {
            add {
                m_OnLateUpdate += value;
#if UNITY_EDITOR
                m_GlobalApplication?.UpdateInfo (value, 1);
#endif
            }
            remove {
                m_OnLateUpdate -= value;
#if UNITY_EDITOR
                m_GlobalApplication?.UpdateInfo (value, 2);
#endif
            }
        }

        public static void RegisterUpdateEvent (UpdateMode mode, Action action) {

            switch (mode) {
                case UpdateMode.Init:
                    break;
                case UpdateMode.Update:
                    OnUpdate += action;
                    break;
                case UpdateMode.FixedUpdate:
                    break;
                case UpdateMode.LateUpdate:
                    break;
            }
        }

        public static void UnregisterUpdateEvent (UpdateMode mode, Action action) {
            switch (mode) {
                case UpdateMode.Init:
                    break;
                case UpdateMode.Update:
                    OnUpdate -= action;
                    break;
                case UpdateMode.FixedUpdate:
                    break;
                case UpdateMode.LateUpdate:
                    break;
            }
        }

        [RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitInRuntime () {
            if (!m_GameObj) {
                m_GameObj = new GameObject ("GameApplication");

#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    GameObject.DontDestroyOnLoad (m_GameObj);

                // m_GameObj.hideFlags =
                //     HideFlags.DontSaveInEditor |
                //     HideFlags.DontSaveInBuild |
                //     HideFlags.DontUnloadUnusedAsset;
            }

            if (!m_GlobalApplication) m_GlobalApplication = m_GameObj.AddComponent<GlobalApplication> ();

        }

#if UNITY_EDITOR
        // Editor Time
        public static float DeltaTimeInEditor { get => m_DeltaTime; }

        private static double m_LastTimeInEditor;
        private static float m_DeltaTime;

        [UnityEditor.InitializeOnLoadMethod]
        private static void InitInEditor () {
            UnityEditor.EditorApplication.update += () => {
                m_DeltaTime = (float) (UnityEditor.EditorApplication.timeSinceStartup - m_LastTimeInEditor);
                m_LastTimeInEditor = UnityEditor.EditorApplication.timeSinceStartup;
            };
        }
#endif

        private static GameObject m_GameObj;
        private static GlobalApplication m_GlobalApplication;

        private static Action m_OnInit;
        private static Action m_OnUpdate;
        private static Action m_OnFixedUpdate;
        private static Action m_OnLateUpdate;

        // #pragma warning disable 0414
        //         private static GlobalApplication m_GlobalApplication = (new GameObject ("GlobalApplication")).AddComponent<GlobalApplication> ();

        // #pragma warning restore 0414
        public class GlobalApplication : MonoBehaviour {

            // Init
            private void Start () {
                m_OnInit?.Invoke ();
            }

            private void Update () {
                m_OnUpdate?.Invoke ();
            }

            private void FixedUpdate () {
                m_OnFixedUpdate?.Invoke ();
            }

            private void LateUpdate () {
                m_OnLateUpdate?.Invoke ();
            }

#if UNITY_EDITOR
            [SerializeField] private List<string> names = new List<string> ();
            [SerializeField] private List<int> counts = new List<int> ();

            private Dictionary<string, int> dic = new Dictionary<string, int> ();

            // type : 1 add, 2 remove
            public void UpdateInfo (Action action, int type) {
                var name = action.Target.GetType () + "." + action.Method.Name;

                if (type == 1) {
                    if (!dic.ContainsKey (name))
                        dic.Add (name, 1);
                    else
                        dic[name]++;
                } else if (type == 2) {
                    if (dic.ContainsKey (name)) {
                        if (dic[name] > 0) dic[name]--;
                    }
                } else {
                    return;
                }

                names.Clear ();
                counts.Clear ();

                names = dic.Keys.ToList ();
                counts = dic.Values.ToList ();
            }

            [CustomEditor (typeof (GlobalApplication))]
            public class GameEntryEditor : Editor {

                private SerializedProperty nameProp;
                private SerializedProperty countProp;

                private void OnEnable () {
                    nameProp = serializedObject.FindProperty ("names");
                    countProp = serializedObject.FindProperty ("counts");
                }

                public override void OnInspectorGUI () {
                    // serializedObject.Update ();

                    if (nameProp.isArray && countProp.isArray) {
                        for (int i = 0; i < nameProp.arraySize; i++) {
                            EditorGUI.LabelField (EditorGUILayout.GetControlRect (), countProp.GetArrayElementAtIndex (i).intValue.ToString ("000") + " " + nameProp.GetArrayElementAtIndex (i).stringValue);
                        }
                    }

                    // serializedObject.ApplyModifiedProperties ();
                }
            }
#endif
        }

    }

    public enum UpdateMode {
        Init,
        Update,
        FixedUpdate,
        LateUpdate
    }

}