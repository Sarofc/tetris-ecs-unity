using System.Collections.Generic;
using UnityEngine;

namespace Saro {
    // TODO
    [TweenAnimation ("Rendering/Material", "Material Property")]
    public partial class TweenMaterialProperty : TweenAnimation {
        public enum Type {
            Color = 0,
            Vector,
            Float,
            Range
        }

        public Renderer targetRenderer;
        public Vector4 from;
        public Vector4 to;

        [SerializeField] private int m_MaterialMask = ~0;
        [SerializeField] private string m_PropertyName;
        [SerializeField] private Type m_PropertyType;

        private int m_PropertyId = -1;

        public string PropertyName {
            get => m_PropertyName;
            private set {
                m_PropertyName = value;
            }
        }
        public int PropertyId {
            get {
                if (m_PropertyId == -1 && !string.IsNullOrEmpty (m_PropertyName)) {
                    m_PropertyId = Shader.PropertyToID (m_PropertyName);
                }
                return m_PropertyId;
            }
            private set {
                m_PropertyId = value;
            }
        }
        public Type PropertyType {
            get => m_PropertyType;
            private set {
                m_PropertyType = value;
            }
        }

        public bool AllMaterialSelected {
            get { return m_MaterialMask == ~0; }
        }

        public bool NoneMaterialSelected {
            get { return m_MaterialMask == 0; }
        }

        public void SelectAllMaterials () {
            m_MaterialMask = ~0;
        }

        public void DeselectAllMaterials () {
            m_MaterialMask = 0;
        }

        public void SetProperty (string name, Type type) {
            m_PropertyName = name;
            PropertyType = type;
            PropertyId = -1;
        }

        public bool IsMaterialSelected (int idx) {
            return (m_MaterialMask & (1 << idx)) != 0;
        }

        public void SetMaterialSelected (int idx, bool selected) {
            if (selected) m_MaterialMask = m_MaterialMask | (1 << idx);
            else m_MaterialMask = m_MaterialMask & (~(1 << idx));
        }

        protected override void OnInterpolate (float f) {
            if (PropertyId != -1 && targetRenderer) {
                // var value = Vector4.LerpUnclamped (m_From, m_To, f);

                var properties = new MaterialPropertyBlock ();
                if (AllMaterialSelected) {
                    targetRenderer.GetPropertyBlock (properties);
                    SetPropertyBlockValue ();
                    targetRenderer.SetPropertyBlock (properties);
                } else {
                    int materialCount = GetMaterials ();
                    for (int i = 0; i < materialCount; i++) {
                        if (IsMaterialSelected (i)) {
                            targetRenderer.GetPropertyBlock (properties, i);
                            SetPropertyBlockValue ();
                            targetRenderer.SetPropertyBlock (properties, i);
                        }
                    }
                }

                void SetPropertyBlockValue () {
                    switch (PropertyType) {
                        case Type.Color:
                        case Type.Vector:
                            properties.SetVector (PropertyId, Vector4.LerpUnclamped (from, to, f));
                            break;
                        case Type.Float:
                        case Type.Range:
                            properties.SetFloat (PropertyId, Mathf.LerpUnclamped (from.x, to.x, f));
                            break;
                    }
                }

            }

        }

        private static List<Material> m_Materials = new List<Material> ();

        private int GetMaterials () {
            m_Materials.Clear ();
            if (targetRenderer) targetRenderer.GetSharedMaterials (m_Materials);
            return Mathf.Min (m_Materials.Count, 32);
        }

        private List<MaterialPropertyBlock> m_TmpBlocks;

    }
}