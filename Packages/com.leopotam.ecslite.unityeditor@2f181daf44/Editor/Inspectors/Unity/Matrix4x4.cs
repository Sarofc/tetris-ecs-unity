// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class Matrix4x4Inspector : EcsComponentInspectorTyped<Matrix4x4>
    {
        protected override bool OnGuiTyped(string label, ref Matrix4x4 value, EcsEntityDebugView entityView)
        {
            var position = EditorGUILayout.GetControlRect(false, 64);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label));

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            const int rowHeight = 16;

            bool changed = false;

            for (int i = 0; i < 4; i++)
            {
                var row = value.GetRow(i);

                var rect = new Rect(position.x, position.y + rowHeight * i, position.width, rowHeight);

                var newRow = EditorGUI.Vector4Field(rect, GUIContent.none, row);

                if (newRow != row)
                {
                    changed = true;
                    value.SetRow(i, newRow);
                }
            }

            EditorGUI.indentLevel = indent;

            return changed;
        }
    }
}