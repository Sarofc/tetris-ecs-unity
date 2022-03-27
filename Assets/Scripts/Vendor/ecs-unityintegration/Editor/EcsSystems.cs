// ----------------------------------------------------------------------------
// The MIT License
// Unity integration https://github.com/Leopotam/ecs-unityintegration
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2021 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.Ecs.UnityIntegration.Editor {
    [CustomEditor (typeof (EcsSystemsObserver))]
    sealed class EcsSystemsObserverInspector : UnityEditor.Editor {
        EcsSystemsObserver _observer;

        public override void OnInspectorGUI () {
            var savedState = GUI.enabled;
            GUI.enabled = true;
            var systemsGroup = _observer.GetSystems ();

            GUILayout.BeginVertical (GUI.skin.box);
            EditorGUILayout.LabelField ("Init systems", EditorStyles.boldLabel);
            OnInitSystemsGUI (systemsGroup);
            GUILayout.EndVertical ();

            GUILayout.BeginVertical (GUI.skin.box);
            EditorGUILayout.LabelField ("Run systems", EditorStyles.boldLabel);
            OnRunSystemsGUI (systemsGroup);
            GUILayout.EndVertical ();

            GUILayout.BeginVertical (GUI.skin.box);
            EditorGUILayout.LabelField ("Destroy systems", EditorStyles.boldLabel);
            OnDestroySystemsGUI (systemsGroup);
            GUILayout.EndVertical ();

            GUI.enabled = savedState;
        }

        void OnEnable () {
            _observer = (EcsSystemsObserver) target;
        }

        void OnDisable () {
            _observer = null;
        }

        void OnInitSystemsGUI (EcsSystems systemsGroup) {
            var systems = systemsGroup.GetAllSystems ();
            EditorGUI.indentLevel++;
            for (var i = 0; i < systems.Count; i++) {
                var item = systems.Items[i];
                if (item is IEcsInitSystem) {
                    var asSystems = item as EcsSystems;
                    EditorGUILayout.LabelField (asSystems != null ? $"[{asSystems.Name ?? asSystems.GetType ().Name}]" : systems.Items[i].GetType ().Name);
                    if (asSystems != null) {
                        OnInitSystemsGUI (asSystems);
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        void OnRunSystemsGUI (EcsSystems systemsGroup) {
            var systems = systemsGroup.GetRunSystems ();
            EditorGUI.indentLevel++;
            for (var i = 0; i < systems.Count; i++) {
                var runItem = systems.Items[i];
                var asSystems = runItem.System as EcsSystems;
                string systemName;
                var type = runItem.System.GetType ();
                if (asSystems != null) {
                    systemName = $"[{asSystems.Name ?? type.Name}]";
                } else {
                    systemName = type.Name;
                    if (type.IsGenericType) {
                        var tilda = systemName.IndexOf ('`');
                        if (tilda > 0) {
                            systemName = systemName.Remove (tilda);
                        }
                        systemName += "<";
                        var args = type.GetGenericArguments ();
                        for (var ii = 0; ii < args.Length; ii++) {
                            // systemName += $",{args[ii].Name}";
                            systemName += ii == 0 ? args[ii].Name : $",{args[ii].Name}";
                        }
                        systemName += ">";
                    }
                }
                runItem.Active = EditorGUILayout.ToggleLeft (systemName, runItem.Active);
                if (asSystems != null && runItem.Active) {
                    OnRunSystemsGUI (asSystems);
                }
            }
            EditorGUI.indentLevel--;
        }

        void OnDestroySystemsGUI (EcsSystems systemsGroup) {
            var systems = systemsGroup.GetAllSystems ();
            EditorGUI.indentLevel++;
            for (var i = 0; i < systems.Count; i++) {
                var item = systems.Items[i];
                if (item is IEcsDestroySystem) {
                    var asSystems = item as EcsSystems;
                    EditorGUILayout.LabelField (asSystems != null ? $"[{asSystems.Name ?? asSystems.GetType ().Name}]" : systems.Items[i].GetType ().Name);
                    if (asSystems != null) {
                        OnDestroySystemsGUI (asSystems);
                    }
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}