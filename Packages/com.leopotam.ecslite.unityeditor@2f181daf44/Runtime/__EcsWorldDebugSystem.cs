//// ----------------------------------------------------------------------------
//// The Proprietary or MIT-Red License
//// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
//// ----------------------------------------------------------------------------

//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Object = UnityEngine.Object;

//namespace Leopotam.EcsLite.UnityEditor
//{
//    public sealed class EcsWorldDebugSystem : IEcsPreInitSystem, IEcsRunSystem, IEcsWorldEventListener
//    {
//        private readonly string m_WorldName;
//        private readonly GameObject m_RootGO;
//        private readonly Transform m_EntitiesRoot;
//        private readonly bool m_BakeComponentsInName;
//        private readonly string m_EntityNameFormat;
//        private EcsWorld m_World;
//        private EcsEntityDebugView[] m_Entities;
//        private Dictionary<int, byte> m_DirtyEntities;
//        private Type[] m_TypesCache;

//        public EcsWorldDebugSystem(string worldName = null, bool bakeComponentsInName = true, string entityNameFormat = "X8")
//        {
//            m_BakeComponentsInName = bakeComponentsInName;
//            m_WorldName = worldName;
//            m_EntityNameFormat = entityNameFormat;
//            m_RootGO = new GameObject(m_WorldName != null ? $"[ECS-WORLD {m_WorldName}]" : "[ECS-WORLD]");
//            Object.DontDestroyOnLoad(m_RootGO);
//            m_RootGO.hideFlags = HideFlags.NotEditable;
//            m_EntitiesRoot = new GameObject("Entities").transform;
//            m_EntitiesRoot.gameObject.hideFlags = HideFlags.NotEditable;
//            m_EntitiesRoot.SetParent(m_RootGO.transform, false);
//        }

//        void IEcsPreInitSystem.PreInit(EcsSystems systems)
//        {
//            // world
//            m_World = systems.GetWorld(m_WorldName);
//            if (m_World == null) { throw new Exception("Cant find required world."); }
//            var worldDebugView = m_RootGO.AddComponent<EcsWorldDebugView>();
//            worldDebugView.ecsWorld = m_World;
//            worldDebugView.debugSystem = this;

//            // systems
//            var systemRootGo = new GameObject("Systems");
//            systemRootGo.transform.parent = m_RootGO.transform;
//            systemRootGo.hideFlags = HideFlags.DontSave;
//            var view = systemRootGo.AddComponent<EcsSystemsDebugView>();
//            view.ecsSystems = systems;
//            view.debugSystem = this;

//            // entities
//            m_EntitiesRoot.transform.hierarchyCapacity = 20480;
//            m_Entities = new EcsEntityDebugView[m_World.GetWorldSize()];
//            m_DirtyEntities = new Dictionary<int, byte>(m_Entities.Length);
//            m_World.AddEventListener(this);
//            var entities = Array.Empty<int>();
//            var entitiesCount = m_World.GetAllEntities(ref entities);
//            for (var i = 0; i < entitiesCount; i++)
//            {
//                ((IEcsWorldEventListener)this).OnEntityCreated(entities[i]);
//            }
//        }

//        void IEcsRunSystem.Run(EcsSystems systems)
//        {
//            foreach (var pair in m_DirtyEntities)
//            {
//                var entity = pair.Key;
//                var entityName = entity.ToString(m_EntityNameFormat);
//                if (m_World.GetEntityGen(entity) > 0)
//                {
//                    var count = m_World.GetComponentTypes(entity, ref m_TypesCache);
//                    for (var i = 0; i < count; i++)
//                    {
//                        entityName = $"{entityName}:{EditorExtensions.GetCleanGenericTypeName(m_TypesCache[i])}";
//                    }
//                }
//                m_Entities[entity].name = entityName;
//            }
//            m_DirtyEntities.Clear();
//        }

//        void IEcsWorldEventListener.OnEntityCreated(int entity)
//        {
//            if (!m_Entities[entity])
//            {
//                var go = new GameObject();
//                go.transform.SetParent(m_EntitiesRoot, false);
//                var entityObserver = go.AddComponent<EcsEntityDebugView>();
//                entityObserver.entity = entity;
//                entityObserver.world = m_World;
//                entityObserver.debugSystem = this;
//                m_Entities[entity] = entityObserver;
//                if (m_BakeComponentsInName)
//                {
//                    m_DirtyEntities[entity] = 1;
//                }
//                else
//                {
//                    go.name = entity.ToString(m_EntityNameFormat);
//                }
//            }
//            m_Entities[entity].gameObject.SetActive(true);
//        }

//        void IEcsWorldEventListener.OnEntityDestroyed(int entity)
//        {
//            if (m_Entities[entity])
//            {
//                m_Entities[entity].gameObject.SetActive(false);
//            }
//        }

//        void IEcsWorldEventListener.OnEntityChanged(int entity)
//        {
//            if (m_BakeComponentsInName)
//            {
//                m_DirtyEntities[entity] = 1;
//            }
//        }

//        void IEcsWorldEventListener.OnFilterCreated(EcsFilter filter) { }

//        void IEcsWorldEventListener.OnWorldResized(int newSize)
//        {
//            Array.Resize(ref m_Entities, newSize);
//        }

//        void IEcsWorldEventListener.OnWorldDestroyed(EcsWorld world)
//        {
//            m_World.RemoveEventListener(this);
//            Object.Destroy(m_RootGO);
//        }

//        public EcsEntityDebugView GetEntityView(int entity)
//        {
//            return entity >= 0 && entity < m_Entities.Length ? m_Entities[entity] : null;
//        }
//    }
//}