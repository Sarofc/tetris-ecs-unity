using System;
using UnityEngine;

namespace Saro {

    [System.AttributeUsage (System.AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TweenAnimationAttribute : System.Attribute {
        // See the attribute guidelines at
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public readonly string menu;
        public readonly string name;

        public TweenAnimationAttribute (string menu, string name) {
            this.menu = menu;
            this.name = name;
        }
    }

    public abstract partial class TweenAnimation : MonoBehaviour {

        [SerializeField] private float m_MinNormalizedTime = 0f;
        [SerializeField] private float m_MaxNormalizedTime = 1f;

        [SerializeField] private CustomizableInterpolator m_Interpolator;

        public float MinNormalizedTime {
            get { return m_MinNormalizedTime; }
            set {
                m_MinNormalizedTime = Mathf.Clamp01 (value);
                m_MaxNormalizedTime = Mathf.Clamp (m_MaxNormalizedTime, m_MinNormalizedTime, 1f);
            }
        }
        public float MaxNormalizedTime {
            get { return m_MaxNormalizedTime; }
            set {
                m_MaxNormalizedTime = Mathf.Clamp01 (value);
                m_MinNormalizedTime = Mathf.Clamp (m_MinNormalizedTime, 0f, m_MaxNormalizedTime);
            }
        }

        public void SetEase (CustomizableInterpolator.Type type) {
            m_Interpolator.type = type;
        }

        // call in NormalizedTime Setter
        public void OnUpdate (float normalizedTime) {
            if (normalizedTime <= m_MinNormalizedTime) normalizedTime = 0f;
            else if (normalizedTime >= m_MaxNormalizedTime) normalizedTime = 1f;
            else normalizedTime = (normalizedTime - m_MinNormalizedTime) / (m_MaxNormalizedTime - m_MinNormalizedTime);

            OnInterpolate (m_Interpolator[normalizedTime]);
        }

        protected abstract void OnInterpolate (float f);
    }
}