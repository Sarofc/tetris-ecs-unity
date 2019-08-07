#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Saro {
    public struct LabelWidthScope : IDisposable {
        private float m_Original;

        public LabelWidthScope (float value) {
            this.m_Original = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = value;
        }

        void IDisposable.Dispose () {
            EditorGUIUtility.labelWidth = m_Original;
        }
    }

    public struct DisabledScope : IDisposable {

        public DisabledScope (bool disabled) {
            EditorGUI.BeginDisabledGroup (disabled);
        }
        void IDisposable.Dispose () {
            EditorGUI.EndDisabledGroup ();
        }
    }

    public struct GUIContentColorScope : IDisposable {
        private Color m_TmpColor;

        public GUIContentColorScope (Color value) {
            this.m_TmpColor = GUI.contentColor;
            GUI.contentColor = value;
        }

        void IDisposable.Dispose () {
            GUI.contentColor = m_TmpColor;
        }
    }

    public struct GUIColorScope : IDisposable {
        private Color m_TmpColor;

        public GUIColorScope (Color value) {
            this.m_TmpColor = GUI.color;
            GUI.color = value;
        }

        void IDisposable.Dispose () {
            GUI.color = m_TmpColor;
        }
    }

    public struct ChangeCheckScope : IDisposable {
        private bool m_End;
        private bool m_Changed;
        private UnityEngine.Object m_UndoRecordTarget;

        public ChangeCheckScope (UnityEngine.Object m_UndoRecordTarget) : this () {
            m_End = false;
            m_Changed = false;
            this.m_UndoRecordTarget = m_UndoRecordTarget;
            EditorGUI.BeginChangeCheck ();
        }

        public bool Changed {
            get {
                if (!m_End) {
                    m_End = true;
                    m_Changed = EditorGUI.EndChangeCheck ();
                    if (m_Changed && m_UndoRecordTarget) {
                        Undo.RecordObject (m_UndoRecordTarget, m_UndoRecordTarget.name);
                    }
                }
                return m_Changed;
            }
        }

        void IDisposable.Dispose () {
            if (!m_End) {
                m_End = true;
                m_Changed = EditorGUI.EndChangeCheck ();
            }
        }
    }

    public struct HandlesColorScope : IDisposable {
        private Color m_Color;

        public HandlesColorScope (Color value) {
            this.m_Color = Handles.color;
            Handles.color = value;
        }

        void IDisposable.Dispose () {
            Handles.color = m_Color;
        }
    }
}
#endif