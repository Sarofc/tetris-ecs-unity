using UnityEngine;

namespace Leopotam.EcsLite.Extension
{
    public class EcsMonoLink : MonoBehaviour, IEcsMonoLink
    {
        public ref EcsPackedEntityWithWorld Entity => ref m_Entity;

        public bool IsAlive => m_Entity.Unpack(out _, out _);

        private EcsPackedEntityWithWorld m_Entity;

        public void Link(in EcsPackedEntityWithWorld ent) => m_Entity = ent;
    }
}
