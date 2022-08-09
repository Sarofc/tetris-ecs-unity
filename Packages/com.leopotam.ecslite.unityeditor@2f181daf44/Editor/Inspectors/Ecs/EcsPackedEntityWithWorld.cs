// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class EcsPackedEntityWithWorldInspector : EcsComponentInspectorTyped<EcsPackedEntityWithWorld>
    {
        protected override bool OnGuiTyped(string label, ref EcsPackedEntityWithWorld value, EcsEntityDebugView entityView)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            if (value.Unpack(out var unpackedWorld, out var unpackedEntity))
            {
                if (unpackedWorld == entityView.world)
                {
                    var ent = entityView.debugSystem.GetEntityView(unpackedEntity);
                    if (GUILayout.Button($"Ping [{ent.entity:x8}]"))
                    {
                        EditorGUIUtility.PingObject(ent);
                    }
                }
                else
                {
                    EditorGUILayout.SelectableLabel("<External entity>", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
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