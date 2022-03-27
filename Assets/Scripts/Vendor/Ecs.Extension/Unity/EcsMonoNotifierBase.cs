using UnityEngine;

namespace Leopotam.Ecs.Extension
{
    public abstract class EcsMonoNotifierBase : MonoBehaviour
    {
        protected ref EcsEntity Entity => ref Provider.Entity;

        private IEcsMonoProvider Provider
        {
            get
            {
                if (m_Provider != null) return m_Provider;
                if (!TryGetComponent(out m_Provider))
                {
                    m_Provider = gameObject.AddComponent<EcsMonoProvider>();
                    Debug.LogError("null provider, create default provider: " + typeof(EcsMonoProvider));
                }
                return m_Provider;
            }
        }

        private IEcsMonoProvider m_Provider;
    }
}
