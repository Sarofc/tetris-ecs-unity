// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Text;

#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Leopotam.EcsLite
{
    public interface IEcsPool
    {
        void Resize(int capacity);
        bool Has(int entity);
        void Del(int entity);
        void AddRaw(int entity, object dataRaw);
        object GetRaw(int entity);
        void SetRaw(int entity, object dataRaw);
        int GetId();
        Type GetComponentType();
    }

    public interface IEcsAutoReset<T> where T : struct
    {
        void AutoReset(ref T c);
    }

#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
    public sealed class EcsPool<T> : IEcsPool where T : struct, IEcsComponent
    {
        private readonly Type m_Type;
        private readonly EcsWorld m_World;
        private readonly int m_Id;
        private readonly AutoResetHandler m_AutoReset;

        // 1-based index.
        private T[] m_DenseItems;
        private int[] m_SparseItems;
        private int m_DenseItemsCount;
        private int[] m_RecycledItems;
        private int m_RecycledItemsCount;
#if ENABLE_IL2CPP && !UNITY_EDITOR
        T m_AutoresetFakeInstance;
#endif

        public override string ToString()
        {
            return $"type: {m_Type.Name} id: {m_Id} denseItem: {m_DenseItems.Length} recycledItems: {m_RecycledItems.Length} sparseItems: {m_SparseItems.Length}";
        }

        internal EcsPool(EcsWorld world, int id, int denseCapacity, int sparseCapacity, int recycledCapacity)
        {
            m_Type = typeof(T);

#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!typeof(IEcsComponent).IsAssignableFrom(m_Type))
            {
                throw new Exception($"{m_Type} is not impl {nameof(IEcsComponent)}");
            }
#endif

            m_World = world;
            m_Id = id;
            m_DenseItems = new T[denseCapacity + 1];
            m_SparseItems = new int[sparseCapacity];
            m_DenseItemsCount = 1;
            m_RecycledItems = new int[recycledCapacity];
            m_RecycledItemsCount = 0;
            var isAutoReset = typeof(IEcsAutoReset<T>).IsAssignableFrom(m_Type);
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!isAutoReset && m_Type.GetInterface("IEcsAutoReset`1") != null)
            {
                throw new Exception($"IEcsAutoReset should have <{m_Type.Name}> constraint for component \"{m_Type.Name}\".");
            }
#endif
            if (isAutoReset)
            {
                var autoResetMethod = m_Type.GetMethod(nameof(IEcsAutoReset<T>.AutoReset));
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                if (autoResetMethod == null)
                {
                    throw new Exception(
                        $"IEcsAutoReset<{m_Type.Name}> explicit implementation not supported, use implicit instead.");
                }
#endif
                m_AutoReset = (AutoResetHandler)Delegate.CreateDelegate(
                    typeof(AutoResetHandler),
#if ENABLE_IL2CPP && !UNITY_EDITOR
                    m_AutoresetFakeInstance,
#else
                    null,
#endif
                    autoResetMethod);
            }
        }

#if UNITY_2020_3_OR_NEWER
        [UnityEngine.Scripting.Preserve]
        private
#endif
        void ReflectionSupportHack()
        {
            m_World.GetPool<T>();
            m_World.Filter().Inc<T>().Exc<T>().End();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsWorld GetWorld()
        {
            return m_World;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetId()
        {
            return m_Id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetComponentType()
        {
            return m_Type;
        }

        void IEcsPool.Resize(int capacity)
        {
            Array.Resize(ref m_SparseItems, capacity);
        }

        object IEcsPool.GetRaw(int entity)
        {
            return Get(entity);
        }

        void IEcsPool.SetRaw(int entity, object dataRaw)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (dataRaw == null || dataRaw.GetType() != m_Type) { throw new Exception("Invalid component data, valid \"{typeof (T).Name}\" instance required."); }
            if (m_SparseItems[entity] <= 0) { throw new Exception($"Component \"{typeof(T).Name}\" not attached to entity."); }
#endif
            m_DenseItems[m_SparseItems[entity]] = (T)dataRaw;
        }

        void IEcsPool.AddRaw(int entity, object dataRaw)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (dataRaw == null || dataRaw.GetType() != m_Type) { throw new Exception("Invalid component data, valid \"{typeof (T).Name}\" instance required."); }
#endif
            ref var data = ref Add(entity);
            data = (T)dataRaw;
        }

        public T[] GetRawDenseItems()
        {
            return m_DenseItems;
        }

        public ref int GetRawDenseItemsCount()
        {
            return ref m_DenseItemsCount;
        }

        public int[] GetRawSparseItems()
        {
            return m_SparseItems;
        }

        public int[] GetRawRecycledItems()
        {
            return m_RecycledItems;
        }

        public ref int GetRawRecycledItemsCount()
        {
            return ref m_RecycledItemsCount;
        }

        public ref T Add(int entity)
        {
            UnityEngine.Debug.LogError("EcsPool::Add 1");

#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!m_World.IsEntityAliveInternal(entity)) { throw new Exception("Cant touch destroyed entity."); }
            //if (_sparseItems[entity] > 0) { throw new Exception ($"Component \"{typeof (T).Name}\" already attached to entity."); }
#endif
            // API 调整
            // 已拥有，就直接返回组件
            if (m_SparseItems[entity] > 0)
            {
                return ref m_DenseItems[m_SparseItems[entity]];
            }

            UnityEngine.Debug.LogError("EcsPool::Add 2");

            int idx;
            if (m_RecycledItemsCount > 0)
            {
                idx = m_RecycledItems[--m_RecycledItemsCount];

                UnityEngine.Debug.LogError("EcsPool::Add 3.0");
            }
            else
            {
                idx = m_DenseItemsCount;
                if (m_DenseItemsCount == m_DenseItems.Length)
                {
                    Array.Resize(ref m_DenseItems, m_DenseItemsCount << 1);
                }
                m_DenseItemsCount++;
                m_AutoReset?.Invoke(ref m_DenseItems[idx]);

                UnityEngine.Debug.LogError("EcsPool::Add 3.1");
            }

            UnityEngine.Debug.LogError("EcsPool::Add 3");

            m_SparseItems[entity] = idx;
            m_World.OnEntityChangeInternal(entity, m_Id, true);
            m_World.entities[entity].componentsCount++;
#if DEBUG || LEOECSLITE_WORLD_EVENTS
            m_World.RaiseEntityChangeEvent(entity);
#endif

            UnityEngine.Debug.LogError("EcsPool::Add 4");

            return ref m_DenseItems[idx];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int entity)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!m_World.IsEntityAliveInternal(entity)) { throw new Exception("Cant touch destroyed entity."); }
            if (m_SparseItems[entity] == 0) { throw new Exception($"Cant get \"{typeof(T).Name}\" component - not attached."); }
#endif
            return ref m_DenseItems[m_SparseItems[entity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int entity)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!m_World.IsEntityAliveInternal(entity)) { throw new Exception("Cant touch destroyed entity."); }
#endif
            return m_SparseItems[entity] > 0;
        }

        public void Del(int entity)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!m_World.IsEntityAliveInternal(entity)) { throw new Exception("Cant touch destroyed entity."); }
#endif
            ref var sparseData = ref m_SparseItems[entity];
            if (sparseData > 0)
            {
                m_World.OnEntityChangeInternal(entity, m_Id, false);
                if (m_RecycledItemsCount == m_RecycledItems.Length)
                {
                    Array.Resize(ref m_RecycledItems, m_RecycledItemsCount << 1);
                }
                m_RecycledItems[m_RecycledItemsCount++] = sparseData;
                if (m_AutoReset != null)
                {
                    m_AutoReset.Invoke(ref m_DenseItems[sparseData]);
                }
                else
                {
                    m_DenseItems[sparseData] = default;
                }
                sparseData = 0;
                ref var entityData = ref m_World.entities[entity];
                entityData.componentsCount--;
#if DEBUG || LEOECSLITE_WORLD_EVENTS
                m_World.RaiseEntityChangeEvent(entity);
#endif
                if (entityData.componentsCount == 0)
                {
                    m_World.DelEntity(entity);
                }
            }
        }

        private delegate void AutoResetHandler(ref T component);
    }
}