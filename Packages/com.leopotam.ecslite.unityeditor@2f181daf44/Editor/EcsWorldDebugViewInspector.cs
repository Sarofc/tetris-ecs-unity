// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor
{
    [CustomEditor(typeof(EcsWorldDebugView))]
    internal sealed class EcsWorldDebugViewInspector : Editor
    {
        private IEcsSystem[] m_Systems;
        private IEcsPool[] m_Pools;
        private bool m_PoolFoldout = false;

        public override void OnInspectorGUI()
        {
            var observer = (EcsWorldDebugView) target;
            if (observer.ecsWorld != null)
            {
                DrawComponents(observer);
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawComponents(EcsWorldDebugView debugView)
        {
            if (debugView.gameObject.activeSelf)
            {
                var world = debugView.ecsWorld;

                EditorGUILayout.LabelField("WorldSize: " + world.GetWorldSize());
                EditorGUILayout.LabelField("EntitiesCount: " + world.GetEntitiesCount());
                EditorGUILayout.LabelField("AllocatedEntitiesCount: " + world.GetAllocatedEntitiesCount());
                EditorGUILayout.LabelField("RawEntitiesCount: " + world.GetRawEntities().Length);
                EditorGUILayout.LabelField("FreeMaskCount: " + world.GetFreeMaskCount());

                EditorGUILayout.Space();
                var poolCount = world.GetAllPools(ref m_Pools);
                m_PoolFoldout = EditorGUILayout.Foldout(m_PoolFoldout, "PoolsCount: " + poolCount);
                if (m_PoolFoldout)
                {
                    var indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < poolCount; i++)
                    {
                        var pool = m_Pools[i];
                        EditorGUILayout.LabelField(pool.ToString());
                    }
                    EditorGUI.indentLevel = indentLevel;
                }
            }
        }
    }
}