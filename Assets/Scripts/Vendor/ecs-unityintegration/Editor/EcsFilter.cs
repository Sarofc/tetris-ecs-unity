// ----------------------------------------------------------------------------
// The MIT License
// Unity integration https://github.com/Leopotam/ecs-unityintegration
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2021 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.Ecs.UnityIntegration.Editor
{
    [CustomEditor(typeof(EcsFilterObserver))]
    internal sealed class EcsFilterObserverInspector : UnityEditor.Editor
    {
        private EcsFilterObserver _observer;

        public override void OnInspectorGUI()
        {
            if (_observer != null)
            {
                var guiEnabled = GUI.enabled;
                GUI.enabled = true;
                DrawComponents();
                GUI.enabled = guiEnabled;
                EditorUtility.SetDirty(target);
            }
        }

        private void OnEnable()
        {
            _observer = target as EcsFilterObserver;
        }

        private void OnDisable()
        {
            _observer = null;
        }

        private void DrawComponents()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            var count = _observer.Filter.GetEntitiesCount();
            EditorGUILayout.LabelField($"Entities: {count}", EditorStyles.boldLabel);
            if (count > 0)
            {
                var ego = _observer.World.EntityGameObjects;
                foreach (var idx in _observer.Filter)
                {
                    ref var entity = ref _observer.Filter.GetEntity(idx);
                    if (entity.IsAlive())
                    {
                        ego.TryGetValue(entity.GetInternalId(), out var entityGo);
                        EditorGUILayout.ObjectField(entityGo, typeof(GameObject), true);
                    }
                }
            }
            GUILayout.EndVertical();
        }
    }
}