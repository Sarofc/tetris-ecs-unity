// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class QuaternionInspector : EcsComponentInspectorTyped<Quaternion>
    {
        protected override bool OnGuiTyped(string label, ref Quaternion value, EcsEntityDebugView entityView)
        {
            var eulerAngles = value.eulerAngles;
            var newValue = EditorGUILayout.Vector3Field(label, eulerAngles);
            if (newValue == eulerAngles) { return false; }
            value = Quaternion.Euler(newValue);
            return true;
        }
    }
}