// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class DoubleInspector : EcsComponentInspectorTyped<double>
    {
        protected override bool OnGuiTyped(string label, ref double value, EcsEntityDebugView entityView)
        {
            var newValue = EditorGUILayout.DoubleField(label, value);
            if (System.Math.Abs(newValue - value) < double.Epsilon) { return false; }
            value = newValue;
            return true;
        }
    }
}