// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class Color32Inspector : EcsComponentInspectorTyped<Color32>
    {
        protected override bool OnGuiTyped(string label, ref Color32 value, EcsEntityDebugView entityView)
        {
            var newValue = EditorGUILayout.ColorField(label, value);
            if (newValue == value) { return false; }
            value = newValue;
            return true;
        }
    }
}