#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {
    public abstract partial class TweenColor : TweenFormTo<Color> {

        public override void Reset () {
            base.Reset ();
            useGradient = false;
            gradient = null;
        }

        protected new abstract class Editor<T> : TweenFormTo<Color>.Editor<T> where T : TweenColor {

            private SerializedProperty m_UseGradientProp;
            private SerializedProperty m_GradientProp;

            private SerializedProperty m_FromAlphaProp;
            private SerializedProperty m_ToAlphaProp;

            protected virtual bool HDR {
                get { return false; }
            }

            protected override void OnEnable () {
                base.OnEnable ();

                m_UseGradientProp = serializedObject.FindProperty ("useGradient");
                m_GradientProp = serializedObject.FindProperty ("gradient");
                m_FromAlphaProp = fromProp.FindPropertyRelative ("a");
                m_ToAlphaProp = toProp.FindPropertyRelative ("a");

            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();

                EditorGUILayout.PropertyField (m_UseGradientProp);

                float labelWidth = EditorGUIUtility.labelWidth;

                if (target.useGradient) {
                    var rect = EditorGUILayout.GetControlRect ();
                    var rect1 = rect;
                    rect.width = labelWidth - 8;
                    rect1.xMin += EditorGUIUtility.labelWidth;

                    EditorGUI.LabelField (rect, "RGBA");
                    EditorGUI.PropertyField (rect1, m_GradientProp, GUIContent.none);

                } else {
                    var content = new GUIContent ();
                    var rect = EditorGUILayout.GetControlRect ();

                    var fromRect = new Rect (rect.x + labelWidth, rect.y, (rect.width - labelWidth) / 2 - 2, rect.height);
                    var toRect = new Rect (rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
                    rect.width = labelWidth - 8;

                    EditorGUI.LabelField (rect, "RGBA");

                    using (new LabelWidthScope (14)) {
                        content.text = "F";
                        fromProp.colorValue = EditorGUI.ColorField (fromRect, content, fromProp.colorValue, true, true, HDR);
                        content.text = "T";
                        toProp.colorValue = EditorGUI.ColorField (toRect, content, toProp.colorValue, false, true, HDR);
                    }

                    // FromToFIeldLayout ("A", m_FromAlphaProp, m_ToAlphaProp, m_ToggleAlphaProp);
                }
            }
        }

    }
}
#endif