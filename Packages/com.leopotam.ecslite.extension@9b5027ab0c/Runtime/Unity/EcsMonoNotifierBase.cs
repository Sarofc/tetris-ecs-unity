using UnityEngine;

namespace Leopotam.EcsLite.Extension
{
    public abstract class EcsMonoNotifierBase : MonoBehaviour
    {
        protected ref EcsPackedEntityWithWorld Entity => ref Link.Entity;

        private IEcsMonoLink Link
        {
            get
            {
                if (m_Link != null) return m_Link;
                if (!TryGetComponent(out m_Link))
                {
                    m_Link = gameObject.AddComponent<EcsMonoLink>();
                    Debug.LogError("null provider, create default provider: " + typeof(EcsMonoLink));
                }
                return m_Link;
            }
        }

        private IEcsMonoLink m_Link;
    }
}
