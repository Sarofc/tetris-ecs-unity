using System;
using System.Collections.Generic;

namespace Leopotam.EcsLite
{
    public class EcsSystemFeature : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem, IEcsPostDestroySystem
    {
        public string FeatureName { get; protected set; }
        public bool Tick { get; set; }

        public IReadOnlyList<IEcsSystem> Systems => m_Systems;

        private readonly List<IEcsSystem> m_Systems;
        private readonly List<IEcsRunSystem> m_RunSystems;

        public EcsSystemFeature(string featureName)
        {
            FeatureName = featureName;
            Tick = true;

            m_Systems = new List<IEcsSystem>();
            m_RunSystems = new List<IEcsRunSystem>();
        }

        public void AddSystem(IEcsSystem system)
        {
            m_Systems.Add(system);
            if (system is IEcsRunSystem _system)
            {
                m_RunSystems.Add(_system);
            }
        }

        public void RemoveSystem(IEcsSystem system)
        {
            m_Systems.Remove(system);
            if (system is IEcsRunSystem _system)
            {
                m_RunSystems.Remove(_system);
            }
        }

        public void PreInit(EcsSystems systems)
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                var system = m_Systems[i];
                if (system is IEcsPreInitSystem _system)
                {
                    _system.PreInit(systems);
                }
            }
        }

        public void Init(EcsSystems systems)
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                var system = m_Systems[i];
                if (system is IEcsInitSystem _system)
                {
                    _system.Init(systems);
                }
            }
        }

        public void Run(EcsSystems systems)
        {
            if (Tick)
            {
                for (int i = 0; i < m_RunSystems.Count; i++)
                {
                    var system = m_RunSystems[i];
                    system.Run(systems);
                }
            }
        }

        public void Destroy(EcsSystems systems)
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                var system = m_Systems[i];
                if (system is IEcsDestroySystem _system)
                {
                    _system.Destroy(systems);
                }
            }
        }

        public void PostDestroy(EcsSystems systems)
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                var system = m_Systems[i];
                if (system is IEcsPostDestroySystem _system)
                {
                    _system.PostDestroy(systems);
                }
            }
        }
    }
}
