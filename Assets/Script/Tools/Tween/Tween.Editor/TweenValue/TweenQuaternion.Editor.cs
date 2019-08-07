#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {

    public abstract partial class TweenQuaternion {

        private Quaternion m_FromQ = Quaternion.identity;
        private Vector3 m_FromEA = Vector3.zero;
        private Quaternion m_ToQ = Quaternion.identity;
        private Vector3 m_ToEA = Vector3.zero;

        protected new abstract class Editor<T> : TweenFormTo<Quaternion>.Editor<T> where T : TweenQuaternion {
            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();
                if (target.m_FromQ != fromProp.quaternionValue) {
                    fromProp.quaternionValue = target.m_FromQ = fromProp.quaternionValue.normalized;
                    target.m_FromEA = target.m_FromQ.eulerAngles;
                }

                if (target.m_ToQ != toProp.quaternionValue) {
                    toProp.quaternionValue = target.m_ToQ = toProp.quaternionValue.normalized;
                    target.m_ToEA = target.m_ToQ.eulerAngles;
                }

                bool3 fromChanged, toChanged;
                FromToFieldLayout ("X", ref target.m_FromEA.x, ref target.m_ToEA.x, out fromChanged.x, out toChanged.x);
                FromToFieldLayout ("Y", ref target.m_FromEA.y, ref target.m_ToEA.y, out fromChanged.y, out toChanged.y);
                FromToFieldLayout ("z", ref target.m_FromEA.z, ref target.m_ToEA.z, out fromChanged.z, out toChanged.z);

                if (fromChanged.AnyTrue) {
                    fromProp.quaternionValue = target.m_FromQ = Quaternion.Euler (target.m_FromEA.x, target.m_FromEA.y, target.m_FromEA.z);
                }

                if (toChanged.AnyTrue) {
                    toProp.quaternionValue = target.m_ToQ = Quaternion.Euler (target.m_ToEA.x, target.m_ToEA.y, target.m_ToEA.z);
                }
            }
        }

    }
}
#endif