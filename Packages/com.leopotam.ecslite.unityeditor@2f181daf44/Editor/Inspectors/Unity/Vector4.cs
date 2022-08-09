// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class Vector4Inspector : EcsComponentInspectorTyped<Vector4>
    {
        protected override bool OnGuiTyped(string label, ref Vector4 value, EcsEntityDebugView entityView)
        {
            var newValue = EditorGUILayout.Vector4Field(label, value);
            if (newValue == value) { return false; }
            value = newValue;
            return true;
        }
    }
}