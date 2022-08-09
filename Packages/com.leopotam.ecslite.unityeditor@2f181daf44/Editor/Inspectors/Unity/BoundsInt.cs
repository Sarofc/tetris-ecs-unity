// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class BoundsIntInspector : EcsComponentInspectorTyped<BoundsInt>
    {
        protected override bool OnGuiTyped(string label, ref BoundsInt value, EcsEntityDebugView entityView)
        {
            var newValue = EditorGUILayout.BoundsIntField(label, value);
            if (newValue == value) { return false; }
            value = newValue;
            return true;
        }
    }
}