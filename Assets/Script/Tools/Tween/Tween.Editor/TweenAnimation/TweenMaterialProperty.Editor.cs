#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Saro {

    public partial class TweenMaterialProperty {
        private Renderer m_Cache;

        public override void Record () {
            m_Cache = targetRenderer;
            if (targetRenderer) {
                if (m_TmpBlocks == null) m_TmpBlocks = new List<MaterialPropertyBlock> ();

                int materialCount = GetMaterials ();
                for (int i = 0; i < materialCount; i++) {
                    m_TmpBlocks.Add (new MaterialPropertyBlock ());
                    targetRenderer.GetPropertyBlock (m_TmpBlocks[i], i);
                }

                m_TmpBlocks.Add (new MaterialPropertyBlock ());
                targetRenderer.GetPropertyBlock (m_TmpBlocks[m_TmpBlocks.Count - 1]);
            }
        }

        public override void Restore () {
            if (m_Cache) {
                var t = targetRenderer;

                targetRenderer.SetPropertyBlock (m_TmpBlocks[m_TmpBlocks.Count - 1].isEmpty ? null : m_TmpBlocks[m_TmpBlocks.Count - 1]);
                int materialCount = GetMaterials ();
                int count = Mathf.Min (m_TmpBlocks.Count - 1, materialCount);
                for (int i = 0; i < count; i++) {
                    targetRenderer.SetPropertyBlock (m_TmpBlocks[i].isEmpty?null : m_TmpBlocks[i]);
                }

                targetRenderer = t;

            }

            m_TmpBlocks.Clear ();
        }

        public override void Reset () {
            base.Reset ();

            targetRenderer = GetComponent<Renderer> ();
            m_MaterialMask = ~0;
            PropertyName = null;
            PropertyId = -1;
            PropertyType = Type.Color;
            from = Color.white;
            to = Color.white;
        }

        private void OnValidate () {
            PropertyId = -1;
        }

        [CustomEditor (typeof (TweenMaterialProperty))]
        protected new class Editor : Editor<TweenMaterialProperty> {
            struct Property {
                public string name;
                public ShaderUtil.ShaderPropertyType type;
            }

            private SerializedProperty m_FromProp;
            private SerializedProperty m_FromXProp;
            private SerializedProperty m_FromYProp;
            private SerializedProperty m_FromZProp;
            private SerializedProperty m_FromWProp;

            private SerializedProperty m_ToProp;
            private SerializedProperty m_ToXProp;
            private SerializedProperty m_ToYProp;
            private SerializedProperty m_ToZProp;
            private SerializedProperty m_ToWProp;

            private StringBuilder m_SB = new StringBuilder ();

            private SerializedProperty m_TargetRendererProp;

            private void DrawMaterialMask (int materialCount) {
                int count = 0;

                if (target.AllMaterialSelected) m_SB.Append ("Apply All");
                else if (target.NoneMaterialSelected) m_SB.Append ("None");
                else {
                    for (int i = 0; i < materialCount; i++) {
                        if (target.IsMaterialSelected (i)) {
                            count++;

                            if (count > 1) {
                                m_SB.Append (", ");
                                if (count == 4) {
                                    m_SB.Append ("...");
                                    break;
                                }
                            }

                            m_SB.Append (i);
                            m_SB.Append (": ");
                            m_SB.Append (m_Materials[i] ? m_Materials[i].name : "(None)");
                        }
                    }
                }

                var rect = EditorGUILayout.GetControlRect ();
                var content = new GUIContent ("Materials");
                rect = EditorGUI.PrefixLabel (rect, content);

                if (GUI.Button (rect, m_SB.ToString (), EditorStyles.layerMaskField)) {
                    GenericMenu menu = new GenericMenu ();

                    menu.AddItem (new GUIContent ("Apply All"), target.AllMaterialSelected, () => {
                        Undo.RecordObject (target, "select Material");
                        target.SelectAllMaterials ();
                    });

                    menu.AddItem (new GUIContent ("None"), target.NoneMaterialSelected, () => {
                        Undo.RecordObject (target, "Select Material");
                        target.DeselectAllMaterials ();
                    });

                    if (materialCount > 0) menu.AddSeparator (string.Empty);

                    for (int i = 0; i < materialCount; i++) {
                        int index = i;
                        menu.AddItem (new GUIContent (index + ": " + (m_Materials[index] ? m_Materials[index].name : "(None)")),
                            target.IsMaterialSelected (index),
                            () => {
                                Undo.RecordObject (target, "Select Material");
                                target.SetMaterialSelected (index, !target.IsMaterialSelected (index));
                            });
                    }

                    menu.DropDown (rect);
                }
                m_SB.Clear ();
            }

            private void DrawProperty (int materialCount) {
                var rect = EditorGUILayout.GetControlRect ();
                rect = EditorGUI.PrefixLabel (rect, new GUIContent ("Property"));

                if (!string.IsNullOrEmpty (target.PropertyName)) {
                    m_SB.Append (target.PropertyName);
                    m_SB.Append (" (");
                    m_SB.Append (target.PropertyType);
                    m_SB.Append (')');
                }

                if (GUI.Button (rect, m_SB.ToString (), EditorStyles.layerMaskField)) {
                    var properties = new HashSet<Property> ();
                    var menu = new GenericMenu ();

                    for (int i = 0; i < materialCount; i++) {
                        if (target.IsMaterialSelected (i) && m_Materials[i] && m_Materials[i].shader) {
                            var shader = m_Materials[i].shader;
                            int count = ShaderUtil.GetPropertyCount (shader);

                            for (int idx = 0; idx < count; idx++) {
                                if (!ShaderUtil.IsShaderPropertyHidden (shader, idx)) {
                                    var prop = new Property {
                                        name = ShaderUtil.GetPropertyName (shader, idx),
                                        type = ShaderUtil.GetPropertyType (shader, idx)
                                    };

                                    if (properties.Contains (prop)) continue;
                                    properties.Add (prop);

                                    string description = ShaderUtil.GetPropertyDescription (shader, idx);

                                    if (prop.type == ShaderUtil.ShaderPropertyType.TexEnv) {
                                        prop.name += "_ST";
                                        prop.type = ShaderUtil.ShaderPropertyType.Vector;
                                        description += " Scale and Offest";
                                    }

                                    m_SB.Clear ();
                                    m_SB.Append (prop.name);
                                    m_SB.Append (" (\"");
                                    m_SB.Append (description);
                                    m_SB.Append ("\", ");
                                    m_SB.Append (prop.type);
                                    m_SB.Append (')');

                                    menu.AddItem (new GUIContent (m_SB.ToString ()),
                                        target.PropertyName == prop.name && target.PropertyType == (Type) (int) prop.type,
                                        () => {
                                            Undo.RecordObject (target, "Select Property");
                                            Type oldType = target.PropertyType;
                                            target.SetProperty (prop.name, (Type) (int) prop.type);

                                            if (oldType != target.PropertyType) {
                                                if (target.PropertyType == Type.Color)
                                                    target.from = target.to = Color.white;

                                                if (target.PropertyType == Type.Float || target.PropertyType == Type.Range)
                                                    target.from.x = target.to.x = 1f;

                                                if (target.PropertyType == Type.Vector) {
                                                    if (prop.name.EndsWith ("_ST"))
                                                        target.from = target.to = new Vector4 (1, 1, 0, 0);
                                                    else
                                                        target.from = target.to = new Vector4 (1, 1, 1, 1);
                                                }
                                            }
                                        });

                                    m_SB.Clear ();
                                }
                            }
                        }
                    }

                    if (properties.Count == 0) menu.AddItem (new GUIContent ("(No Valid Property)"), false, () => { });

                    menu.DropDown (rect);
                }

                m_SB.Clear ();
            }

            protected override void OnEnable () {
                base.OnEnable ();

                m_TargetRendererProp = serializedObject.FindProperty ("targetRenderer");

                m_FromProp = serializedObject.FindProperty ("from");
                m_FromXProp = m_FromProp.FindPropertyRelative ("x");
                m_FromYProp = m_FromProp.FindPropertyRelative ("y");
                m_FromZProp = m_FromProp.FindPropertyRelative ("z");
                m_FromWProp = m_FromProp.FindPropertyRelative ("w");

                m_ToProp = serializedObject.FindProperty ("to");
                m_ToXProp = m_ToProp.FindPropertyRelative ("x");
                m_ToYProp = m_ToProp.FindPropertyRelative ("y");
                m_ToZProp = m_ToProp.FindPropertyRelative ("z");
                m_ToWProp = m_ToProp.FindPropertyRelative ("w");
            }

            protected override void InitOptionsMenu (GenericMenu menu, Tween tween) {
                base.InitOptionsMenu (menu, tween);

                menu.AddSeparator (string.Empty);

                menu.AddItem (new GUIContent ("Swap From and To"), false, () => {
                    Undo.RecordObject (target, "Swap From and To");

                    var tmp = target.from;
                    target.from = target.to;
                    target.to = tmp;
                });
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();

                EditorGUILayout.PropertyField (m_TargetRendererProp);

                int materialCount = target.GetMaterials ();
                DrawMaterialMask (materialCount);
                DrawProperty (materialCount);

                EditorGUILayout.Space ();

                switch (target.PropertyType) {
                    case Type.Float:
                    case Type.Range:
                        FromToFieldLayout ("Value", m_FromXProp, m_ToXProp);
                        break;

                    case Type.Vector:
                        FromToFieldLayout ("X", m_FromXProp, m_ToXProp);
                        FromToFieldLayout ("Y", m_FromYProp, m_ToYProp);
                        FromToFieldLayout ("Z", m_FromZProp, m_ToZProp);
                        FromToFieldLayout ("W", m_FromWProp, m_ToWProp);
                        break;

                    case Type.Color:
                        var rect = EditorGUILayout.GetControlRect ();
                        float labelWidth = EditorGUIUtility.labelWidth;

                        var fromRect = new Rect (rect.x + labelWidth, rect.y, (rect.width - labelWidth) / 2 - 2, rect.height);
                        var toRect = new Rect (rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
                        rect.width = labelWidth - 8;

                        EditorGUI.LabelField (rect, "Color");

                        using (new LabelWidthScope (14)) {
                            var content = new GUIContent ();
                            content.text = "F";
                            m_FromProp.vector4Value = EditorGUI.ColorField (fromRect, content, m_FromProp.vector4Value, false, true, true);
                            content.text = "T";
                            m_ToProp.vector4Value = EditorGUI.ColorField (toRect, content, m_ToProp.vector4Value, false, true, true);
                        }
                        break;
                }
            }
        }
    }
}
#endif