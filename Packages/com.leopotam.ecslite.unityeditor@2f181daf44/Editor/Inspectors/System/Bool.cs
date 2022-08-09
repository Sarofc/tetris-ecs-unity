// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class BoolInspector : EcsComponentInspectorTyped<bool>
    {
        protected override bool OnGuiTyped(string label, ref bool value, EcsEntityDebugView entityView)
        {
            var newValue = EditorGUILayout.Toggle(label, value);
            if (newValue == value) { return false; }
            value = newValue;
            return true;
        }
    }
}