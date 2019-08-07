#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {

    public abstract partial class TweenVector2 : TweenFormTo<Vector2> {
        protected new abstract class Editor<T> : TweenFormTo<Vector2>.Editor<T> where T : TweenVector2 {

            private SerializedProperty m_FromXProp;
            private SerializedProperty m_FromYProp;
            private SerializedProperty m_ToXProp;
            private SerializedProperty m_ToYProp;

            protected override void OnEnable () {
                base.OnEnable ();

                m_FromXProp = fromProp.FindPropertyRelative ("x");
                m_FromYProp = fromProp.FindPropertyRelative ("y");

                m_ToXProp = toProp.FindPropertyRelative ("x");
                m_ToYProp = toProp.FindPropertyRelative ("y");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();

                FromToFieldLayout ("X", m_FromXProp, m_ToXProp);
                FromToFieldLayout ("Y", m_FromYProp, m_ToYProp);
            }
        }
    }
}
#endif