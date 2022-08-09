// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class EcsPackedEntityInspector : EcsComponentInspectorTyped<EcsPackedEntity>
    {
        protected override bool OnGuiTyped(string label, ref EcsPackedEntity value, EcsEntityDebugView entityView)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            if (value.Unpack(entityView.world, out var unpackedEntity))
            {
                var ent = entityView.debugSystem.GetEntityView(unpackedEntity);
                if (GUILayout.Button($"Ping [{ent.entity:x8}]"))
                {
                    EditorGUIUtility.PingObject(ent);
                }
            }
            else
            {
                if (value.EqualsTo(default))
                {
                    EditorGUILayout.SelectableLabel("<Empty entity>", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                }
                else
                {
                    EditorGUILayout.SelectableLabel("<Invalid entity>", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                }
            }

            EditorGUILayout.EndHorizontal();
            return false;
        }
    }
}