using UnityEngine;

namespace Leopotam.Ecs.Extension
{
    public class EcsMonoProvider : MonoBehaviour, IEcsMonoProvider
    {
        public ref EcsEntity Entity => ref m_Entity;

        public bool IsAlive => m_Entity.IsAlive();

        private EcsEntity m_Entity;

        public void Link(in EcsEntity ent) => m_Entity = ent;
    }
}
