// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using Saro.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Leopotam.EcsLite
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public partial class EcsWorld
    {
        internal EntityData[] entities;
        private int m_EntitiesCount;
        private int[] m_RecycledEntities;
        private int m_RecycledEntitiesCount;
        private IEcsPool[] m_Pools;
        private int m_PoolsCount;
        private readonly int m_PoolDenseSize;
        private readonly int m_PoolRecycledSize;
        private readonly Dictionary<Type, IEcsPool> m_PoolHashes;
        private readonly Dictionary<int, EcsFilter> m_HashedFilters;
        private readonly List<EcsFilter> m_AllFilters;
        private List<EcsFilter>[] m_FiltersByIncludedComponents;
        private List<EcsFilter>[] m_FiltersByExcludedComponents;
        private Mask[] m_Masks;
        private int m_FreeMasksCount;
        private bool m_Destroyed;

#if DEBUG || LEOECSLITE_WORLD_EVENTS
        internal readonly List<EcsSystems> ecsSystemsList = new();
#endif

#if DEBUG || LEOECSLITE_WORLD_EVENTS
        private readonly List<IEcsWorldEventListener> m_EventListeners;

        public void AddEventListener(IEcsWorldEventListener listener)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (listener == null)
            {
                throw new Exception("Listener is null.");
            }
#endif
            m_EventListeners.Add(listener);
        }

        public void RemoveEventListener(IEcsWorldEventListener listener)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (listener == null)
            {
                throw new Exception("Listener is null.");
            }
#endif
            m_EventListeners.Remove(listener);
        }

        public void RaiseEntityChangeEvent(int entity)
        {
            for (int ii = 0, iMax = m_EventListeners.Count; ii < iMax; ii++)
            {
                m_EventListeners[ii].OnEntityChanged(entity);
            }
        }
#endif
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
        private readonly List<int> m_LeakedEntities = new List<int>(512);

        internal bool CheckForLeakedEntities()
        {
            if (m_LeakedEntities.Count > 0)
            {
                for (int i = 0, iMax = m_LeakedEntities.Count; i < iMax; i++)
                {
                    ref var entityData = ref entities[m_LeakedEntities[i]];
                    if (entityData.gen > 0 && entityData.componentsCount == 0)
                    {
                        return true;
                    }
                }

                m_LeakedEntities.Clear();
            }

            return false;
        }
#endif

        public EcsWorld(in Config cfg = default)
        {
            // entities.
            var capacity = cfg.entities > 0 ? cfg.entities : Config.k_EntitiesDefault;
            entities = new EntityData[capacity];
            capacity = cfg.recycledEntities > 0 ? cfg.recycledEntities : Config.k_RecycledEntitiesDefault;
            m_RecycledEntities = new int[capacity];
            m_EntitiesCount = 0;
            m_RecycledEntitiesCount = 0;
            // pools.
            capacity = cfg.pools > 0 ? cfg.pools : Config.k_PoolsDefault;
            m_Pools = new IEcsPool[capacity];
            m_PoolHashes = new Dictionary<Type, IEcsPool>(capacity);
            m_FiltersByIncludedComponents = new List<EcsFilter>[capacity];
            m_FiltersByExcludedComponents = new List<EcsFilter>[capacity];
            m_PoolDenseSize = cfg.poolDenseSize > 0 ? cfg.poolDenseSize : Config.k_PoolDenseSizeDefault;
            m_PoolRecycledSize = cfg.poolRecycledSize > 0 ? cfg.poolRecycledSize : Config.k_PoolRecycledSizeDefault;
            m_PoolsCount = 0;
            // filters.
            capacity = cfg.filters > 0 ? cfg.filters : Config.k_FiltersDefault;
            m_HashedFilters = new Dictionary<int, EcsFilter>(capacity);
            m_AllFilters = new List<EcsFilter>(capacity);
            // masks.
            m_Masks = new Mask[64];
            m_FreeMasksCount = 0;
#if DEBUG || LEOECSLITE_WORLD_EVENTS
            m_EventListeners = new List<IEcsWorldEventListener>(4);
#endif
            m_Destroyed = false;
        }

        public void Destroy()
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (CheckForLeakedEntities())
            {
                throw new Exception($"Empty entity detected before EcsWorld.Destroy().");
            }
#endif
            m_Destroyed = true;
            for (var i = m_EntitiesCount - 1; i >= 0; i--)
            {
                ref var entityData = ref entities[i];
                if (entityData.componentsCount > 0)
                {
                    DelEntity(i);
                }
            }

            m_Pools = Array.Empty<IEcsPool>();
            m_PoolHashes.Clear();
            m_HashedFilters.Clear();
            m_AllFilters.Clear();
            m_FiltersByIncludedComponents = Array.Empty<List<EcsFilter>>();
            m_FiltersByExcludedComponents = Array.Empty<List<EcsFilter>>();
#if DEBUG || LEOECSLITE_WORLD_EVENTS
            for (var ii = m_EventListeners.Count - 1; ii >= 0; ii--)
            {
                m_EventListeners[ii].OnWorldDestroyed(this);
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive()
        {
            return !m_Destroyed;
        }

        public int NewEntity()
        {
            int entity;
            if (m_RecycledEntitiesCount > 0)
            {
                entity = m_RecycledEntities[--m_RecycledEntitiesCount];
                ref var entityData = ref entities[entity];
                entityData.gen = (short) -entityData.gen;
            }
            else
            {
                // new entity.
                if (m_EntitiesCount == entities.Length)
                {
                    // resize entities and component pools.
                    var newSize = m_EntitiesCount << 1;
                    Array.Resize(ref entities, newSize);
                    for (int i = 0, iMax = m_PoolsCount; i < iMax; i++)
                    {
                        m_Pools[i].Resize(newSize);
                    }

                    for (int i = 0, iMax = m_AllFilters.Count; i < iMax; i++)
                    {
                        m_AllFilters[i].ResizeSparseIndex(newSize);
                    }
#if DEBUG || LEOECSLITE_WORLD_EVENTS
                    for (int ii = 0, iMax = m_EventListeners.Count; ii < iMax; ii++)
                    {
                        m_EventListeners[ii].OnWorldResized(newSize);
                    }
#endif
                }

                entity = m_EntitiesCount++;
                entities[entity].gen = 1;
            }
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            m_LeakedEntities.Add(entity);
#endif
#if DEBUG || LEOECSLITE_WORLD_EVENTS
            for (int ii = 0, iMax = m_EventListeners.Count; ii < iMax; ii++)
            {
                m_EventListeners[ii].OnEntityCreated(entity);
            }
#endif
            return entity;
        }

        public void DelEntity(int entity)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (entity < 0 || entity >= m_EntitiesCount)
            {
                throw new Exception("Cant touch destroyed entity.");
            }
#endif
            ref var entityData = ref entities[entity];
            if (entityData.gen < 0)
            {
                return;
            }

            // kill components.
            if (entityData.componentsCount > 0)
            {
                var idx = 0;
                while (entityData.componentsCount > 0 && idx < m_PoolsCount)
                {
                    for (; idx < m_PoolsCount; idx++)
                    {
                        if (m_Pools[idx].Has(entity))
                        {
                            m_Pools[idx++].Del(entity);
                            break;
                        }
                    }
                }
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                if (entityData.componentsCount != 0)
                {
                    throw new Exception(
                        $"Invalid components count on entity {entity} => {entityData.componentsCount}.");
                }
#endif
                return;
            }

            entityData.gen = (short) (entityData.gen == short.MaxValue ? -1 : -(entityData.gen + 1));
            if (m_RecycledEntitiesCount == m_RecycledEntities.Length)
            {
                Array.Resize(ref m_RecycledEntities, m_RecycledEntitiesCount << 1);
            }

            m_RecycledEntities[m_RecycledEntitiesCount++] = entity;
#if DEBUG || LEOECSLITE_WORLD_EVENTS
            for (int ii = 0, iMax = m_EventListeners.Count; ii < iMax; ii++)
            {
                m_EventListeners[ii].OnEntityDestroyed(entity);
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetComponentsCount(int entity)
        {
            return entities[entity].componentsCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short GetEntityGen(int entity)
        {
            return entities[entity].gen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAllocatedEntitiesCount()
        {
            return m_EntitiesCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetWorldSize()
        {
            return entities.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetPoolsCount()
        {
            return m_PoolsCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEntitiesCount()
        {
            return m_EntitiesCount - m_RecycledEntitiesCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityData[] GetRawEntities()
        {
            return entities;
        }

        public int GetFreeMaskCount()
        {
            return m_FreeMasksCount;
        }

        public EcsPool<T> GetPool<T>() where T : struct, IEcsComponent
        {
            return GetPool<T>(m_PoolDenseSize, m_PoolRecycledSize);
        }

        public EcsPool<T> GetPool<T>(int denseCapacity, int recycledCapacity) where T : struct, IEcsComponent
        {
            var poolType = typeof(T);
            if (m_PoolHashes.TryGetValue(poolType, out var rawPool))
            {
                return (EcsPool<T>) rawPool;
            }

            var pool = new EcsPool<T>(this, m_PoolsCount, denseCapacity, entities.Length, recycledCapacity);
            m_PoolHashes[poolType] = pool;
            if (m_PoolsCount == m_Pools.Length)
            {
                var newSize = m_PoolsCount << 1;
                Array.Resize(ref m_Pools, newSize);
                Array.Resize(ref m_FiltersByIncludedComponents, newSize);
                Array.Resize(ref m_FiltersByExcludedComponents, newSize);
            }

            m_Pools[m_PoolsCount++] = pool;
            return pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEcsPool GetPoolById(int typeId)
        {
            return typeId >= 0 && typeId < m_PoolsCount ? m_Pools[typeId] : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEcsPool GetPoolByType(Type type)
        {
            return m_PoolHashes.TryGetValue(type, out var pool) ? pool : null;
        }

        public int GetAllEntities(ref int[] entities)
        {
            var count = m_EntitiesCount - m_RecycledEntitiesCount;
            if (entities == null || entities.Length < count)
            {
                entities = new int[count];
            }

            var id = 0;
            for (int i = 0, iMax = m_EntitiesCount; i < iMax; i++)
            {
                ref var entityData = ref this.entities[i];
                // should we skip empty entities here?
                if (entityData.gen > 0 && entityData.componentsCount >= 0)
                {
                    entities[id++] = i;
                }
            }

            return count;
        }

        public int GetAllPools(ref IEcsPool[] pools)
        {
            var count = m_PoolsCount;
            if (pools == null || pools.Length < count)
            {
                pools = new IEcsPool[count];
            }

            Array.Copy(m_Pools, 0, pools, 0, m_PoolsCount);
            return m_PoolsCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Mask Filter()
        {
            return m_FreeMasksCount > 0 ? m_Masks[--m_FreeMasksCount] : new Mask(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Obsolete("use Filter() instead", true)]
        public Mask Filter<T>() where T : struct, IEcsComponent
        {
            var mask = m_FreeMasksCount > 0 ? m_Masks[--m_FreeMasksCount] : new Mask(this);
            return mask.Inc<T>();
        }

        public int GetComponents(int entity, ref object[] list)
        {
            var itemsCount = entities[entity].componentsCount;
            if (itemsCount == 0)
            {
                return 0;
            }

            if (list == null || list.Length < itemsCount)
            {
                list = new object[m_Pools.Length];
            }

            for (int i = 0, j = 0, iMax = m_PoolsCount; i < iMax; i++)
            {
                if (m_Pools[i].Has(entity))
                {
                    list[j++] = m_Pools[i].GetRaw(entity);
                }
            }

            return itemsCount;
        }

        public int GetComponentTypes(int entity, ref Type[] list)
        {
            var itemsCount = entities[entity].componentsCount;
            if (itemsCount == 0)
            {
                return 0;
            }

            if (list == null || list.Length < itemsCount)
            {
                list = new Type[m_Pools.Length];
            }

            for (int i = 0, j = 0, iMax = m_PoolsCount; i < iMax; i++)
            {
                if (m_Pools[i].Has(entity))
                {
                    list[j++] = m_Pools[i].GetComponentType();
                }
            }

            return itemsCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsEntityAliveInternal(int entity) // TODO  public ?
        {
            return entity >= 0 && entity < m_EntitiesCount && entities[entity].gen > 0;
        }

        private (EcsFilter, bool) GetFilterInternal(Mask mask, int capacity = 512)
        {
            var hash = mask.hash;
            var exists = m_HashedFilters.TryGetValue(hash, out var filter);
            if (exists)
            {
                return (filter, false);
            }

            filter = new EcsFilter(this, mask, capacity, entities.Length);
            m_HashedFilters[hash] = filter;
            m_AllFilters.Add(filter);
            // add to component dictionaries for fast compatibility scan.
            for (int i = 0, iMax = mask.includeCount; i < iMax; i++)
            {
                var list = m_FiltersByIncludedComponents[mask.include[i]];
                if (list == null)
                {
                    list = new List<EcsFilter>(8);
                    m_FiltersByIncludedComponents[mask.include[i]] = list;
                }

                list.Add(filter);
            }

            for (int i = 0, iMax = mask.excludeCount; i < iMax; i++)
            {
                var list = m_FiltersByExcludedComponents[mask.exclude[i]];
                if (list == null)
                {
                    list = new List<EcsFilter>(8);
                    m_FiltersByExcludedComponents[mask.exclude[i]] = list;
                }

                list.Add(filter);
            }

            // scan exist entities for compatibility with new filter.
            for (int i = 0, iMax = m_EntitiesCount; i < iMax; i++)
            {
                ref var entityData = ref entities[i];
                if (entityData.componentsCount > 0 && IsMaskCompatible(mask, i))
                {
                    filter.AddEntity(i);
                }
            }
#if DEBUG || LEOECSLITE_WORLD_EVENTS
            for (int ii = 0, iMax = m_EventListeners.Count; ii < iMax; ii++)
            {
                m_EventListeners[ii].OnFilterCreated(filter);
            }
#endif
            return (filter, true);
        }

        public void OnEntityChangeInternal(int entity, int componentType, bool added)
        {
            var includeList = m_FiltersByIncludedComponents[componentType];
            var excludeList = m_FiltersByExcludedComponents[componentType];
            if (added)
            {
                // add component.
                if (includeList != null)
                {
                    foreach (var filter in includeList)
                    {
                        if (IsMaskCompatible(filter.GetMask(), entity))
                        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                            if (filter.SparseEntities[entity] > 0)
                            {
                                throw new Exception("Entity already in filter.");
                            }
#endif
                            filter.AddEntity(entity);
                        }
                    }
                }

                if (excludeList != null)
                {
                    foreach (var filter in excludeList)
                    {
                        if (IsMaskCompatibleWithout(filter.GetMask(), entity, componentType))
                        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                            if (filter.SparseEntities[entity] == 0)
                            {
                                throw new Exception("Entity not in filter.");
                            }
#endif
                            filter.RemoveEntity(entity);
                        }
                    }
                }
            }
            else
            {
                // remove component.
                if (includeList != null)
                {
                    foreach (var filter in includeList)
                    {
                        if (IsMaskCompatible(filter.GetMask(), entity))
                        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                            if (filter.SparseEntities[entity] == 0)
                            {
                                throw new Exception("Entity not in filter.");
                            }
#endif
                            filter.RemoveEntity(entity);
                        }
                    }
                }

                if (excludeList != null)
                {
                    foreach (var filter in excludeList)
                    {
                        if (IsMaskCompatibleWithout(filter.GetMask(), entity, componentType))
                        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                            if (filter.SparseEntities[entity] > 0)
                            {
                                throw new Exception("Entity already in filter.");
                            }
#endif
                            filter.AddEntity(entity);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsMaskCompatible(Mask filterMask, int entity)
        {
            for (int i = 0, iMax = filterMask.includeCount; i < iMax; i++)
            {
                if (!m_Pools[filterMask.include[i]].Has(entity))
                {
                    return false;
                }
            }

            for (int i = 0, iMax = filterMask.excludeCount; i < iMax; i++)
            {
                if (m_Pools[filterMask.exclude[i]].Has(entity))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsMaskCompatibleWithout(Mask filterMask, int entity, int componentId)
        {
            for (int i = 0, iMax = filterMask.includeCount; i < iMax; i++)
            {
                var typeId = filterMask.include[i];
                if (typeId == componentId || !m_Pools[typeId].Has(entity))
                {
                    return false;
                }
            }

            for (int i = 0, iMax = filterMask.excludeCount; i < iMax; i++)
            {
                var typeId = filterMask.exclude[i];
                if (typeId != componentId && m_Pools[typeId].Has(entity))
                {
                    return false;
                }
            }

            return true;
        }

        public struct Config
        {
            public int entities;
            public int recycledEntities;
            public int pools;
            public int filters;
            public int poolDenseSize;
            public int poolRecycledSize;

            internal const int k_EntitiesDefault = 512;
            internal const int k_RecycledEntitiesDefault = 512;
            internal const int k_PoolsDefault = 512;
            internal const int k_FiltersDefault = 512;
            internal const int k_PoolDenseSizeDefault = 512;
            internal const int k_PoolRecycledSizeDefault = 512;
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
        public sealed partial class Mask
        {
            private readonly EcsWorld m_World;
            internal int[] include;
            internal int[] exclude;
            internal int includeCount;
            internal int excludeCount;
            internal int hash;

#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            private bool m_Built;
#endif

            internal Mask(EcsWorld world)
            {
                m_World = world;
                include = new int[8];
                exclude = new int[2];
                Reset();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Reset()
            {
                includeCount = 0;
                excludeCount = 0;
                hash = 0;
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                m_Built = false;
#endif
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Inc<T>() where T : struct, IEcsComponent
            {
                var poolId = m_World.GetPool<T>().GetId();
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                if (m_Built)
                {
                    throw new Exception("Cant change built mask.");
                }

                if (Array.IndexOf(include, poolId, 0, includeCount) != -1)
                {
                    throw new Exception($"{typeof(T).Name} already in constraints list.");
                }

                if (Array.IndexOf(exclude, poolId, 0, excludeCount) != -1)
                {
                    throw new Exception($"{typeof(T).Name} already in constraints list.");
                }
#endif
                if (includeCount == include.Length)
                {
                    Array.Resize(ref include, includeCount << 1);
                }

                include[includeCount++] = poolId;
                return this;
            }

#if UNITY_2020_3_OR_NEWER
            [UnityEngine.Scripting.Preserve]
#endif
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Exc<T>() where T : struct, IEcsComponent
            {
                var poolId = m_World.GetPool<T>().GetId();
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                if (m_Built)
                {
                    throw new Exception("Cant change built mask.");
                }

                if (Array.IndexOf(include, poolId, 0, includeCount) != -1)
                {
                    throw new Exception($"{typeof(T).Name} already in constraints list.");
                }

                if (Array.IndexOf(exclude, poolId, 0, excludeCount) != -1)
                {
                    throw new Exception($"{typeof(T).Name} already in constraints list.");
                }
#endif
                if (excludeCount == exclude.Length)
                {
                    Array.Resize(ref exclude, excludeCount << 1);
                }

                exclude[excludeCount++] = poolId;
                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public EcsFilter End(int capacity = 512)
            {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                if (m_Built)
                {
                    throw new Exception("Cant change built mask.");
                }

                m_Built = true;
#endif

                //Array.Sort(Include, 0, IncludeCount);
                //Array.Sort(Exclude, 0, ExcludeCount);
                ArrayUtility.Sort(include, 0, includeCount);
                ArrayUtility.Sort(exclude, 0, excludeCount);

                // calculate hash.
                hash = includeCount + excludeCount;
                for (int i = 0, iMax = includeCount; i < iMax; i++)
                {
                    hash = unchecked(hash * 314159 + include[i]);
                }

                for (int i = 0, iMax = excludeCount; i < iMax; i++)
                {
                    hash = unchecked(hash * 314159 - exclude[i]);
                }

                var (filter, isNew) = m_World.GetFilterInternal(this, capacity);
                if (!isNew)
                {
                    Recycle();
                }

                return filter;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Recycle()
            {
                Reset();
                if (m_World.m_FreeMasksCount == m_World.m_Masks.Length)
                {
                    Array.Resize(ref m_World.m_Masks, m_World.m_FreeMasksCount << 1);
                }

                m_World.m_Masks[m_World.m_FreeMasksCount++] = this;
            }
        }

        public struct EntityData
        {
            public short gen;
            public short componentsCount;
        }
    }

#if DEBUG || LEOECSLITE_WORLD_EVENTS
    public interface IEcsWorldEventListener
    {
        void OnEntityCreated(int entity);
        void OnEntityChanged(int entity);
        void OnEntityDestroyed(int entity);
        void OnFilterCreated(EcsFilter filter);
        void OnWorldResized(int newSize);
        void OnWorldDestroyed(EcsWorld world);
    }
#endif
}

#if ENABLE_IL2CPP
// Unity IL2CPP performance optimization attribute.
namespace Unity.IL2CPP.CompilerServices
{
    enum Option
    {
        NullChecks = 1,
        ArrayBoundsChecks = 2
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited =
        false, AllowMultiple = true)]
    class Il2CppSetOptionAttribute : Attribute
    {
        public Option Option { get; private set; }
        public object Value { get; private set; }

        public Il2CppSetOptionAttribute(Option option, object value)
        {
            Option = option;
            Value = value;
        }
    }
}
#endif