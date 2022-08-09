// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor
{
    [CustomEditor(typeof(EcsSystemsDebugView))]
    internal sealed class EcsSystemsDebugViewInspector : Editor
    {
        private IEcsSystem[] m_Systems;

        public override void OnInspectorGUI()
        {
            var observer = (EcsSystemsDebugView)target;
            if (observer.ecsSystemsList != null)
            {
                DrawComponents(observer);
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawComponents(EcsSystemsDebugView debugView)
        {
            if (debugView.gameObject.activeSelf)
            {
                var ecsSytemsList = debugView.ecsSystemsList;

                for (int ii = 0; ii < ecsSytemsList.Count; ii++)
                {
                    var systems = ecsSytemsList[ii];

                    EditorGUILayout.BeginVertical("helpbox");
                    {
                        EditorGUILayout.LabelField($"{systems.SystemsLabel}:", EditorStyles.boldLabel);

                        var systemNum = systems.GetAllSystems(ref m_Systems);
                        for (int i = 0; i < systemNum; i++)
                        {
                            var system = m_Systems[i];
                            // feature
                            if (system is EcsSystemFeature _feature)
                            {
                                EditorGUILayout.BeginVertical("box");
                                {
                                    var rect = EditorGUILayout.GetControlRect();
                                    var toggeRect = rect;
                                    const float k_ToggleWidth = 15f;
                                    toggeRect.width = k_ToggleWidth;
                                    _feature.Tick = EditorGUI.Toggle(toggeRect, _feature.Tick);

                                    var featureRect = rect;
                                    featureRect.x += k_ToggleWidth;
                                    featureRect.width = rect.width - k_ToggleWidth;
                                    EditorGUI.LabelField(featureRect, _feature.FeatureName);

                                    EditorGUI.indentLevel += 2;
                                    for (int k = 0; k < _feature.Systems.Count; k++)
                                    {
                                        var subSystem = _feature.Systems[k];
                                        EditorGUILayout.LabelField(subSystem.GetType().Name);
                                    }
                                    EditorGUI.indentLevel -= 2;
                                }
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                EditorGUILayout.LabelField(system.GetType().Name);
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    if (ii < ecsSytemsList.Count - 1)
                    {
                        EditorGUILayout.Space();
                    }
                }
            }
        }
    }
}