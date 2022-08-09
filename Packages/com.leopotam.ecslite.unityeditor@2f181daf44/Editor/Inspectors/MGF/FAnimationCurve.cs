using Saro.Utility;
using UnityEditor;

namespace Leopotam.EcsLite.UnityEditor.Inspectors
{
    internal sealed class FAnimationCurveInspector : EcsComponentInspectorTyped<FAnimationCurve>
    {
        protected override bool OnGuiTyped(string label, ref FAnimationCurve value, EcsEntityDebugView entityView)
        {
            // TODO 只要点开了inspector，这里每帧都会进来，频繁分配内存，可忽略？
            UnityEngine.AnimationCurve curve = value;
            var newValue = EditorGUILayout.CurveField(label, curve);
            if (newValue == curve) { return false; }
            value = newValue;
            return true;
        }
    }
}