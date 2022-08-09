// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using System.Runtime.CompilerServices;

#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Leopotam.EcsLite
{
    public readonly struct EcsPackedEntity
    {
        public static readonly EcsPackedEntity k_Null = default;
        public readonly int id;
        internal readonly int gen;

        public EcsPackedEntity(int id, int gen)
        {
            this.gen = gen;
            this.id = id;
        }
    }

    public readonly struct EcsPackedEntityWithWorld
    {
        public static readonly EcsPackedEntityWithWorld k_Null = default;
        public readonly int id;
        internal readonly int gen;
        internal readonly EcsWorld world;

        public EcsPackedEntityWithWorld(int id, int gen = 0, EcsWorld world = null)
        {
            this.id = id;
            this.gen = gen;
            this.world = world;
        }
#if DEBUG
        // For using in IDE debugger.
        internal object[] DebugComponentsView
        {
            get
            {
                object[] list = null;
                if (world != null && world.IsAlive() && world.IsEntityAliveInternal(id) && world.GetEntityGen(id) == gen)
                {
                    world.GetComponents(id, ref list);
                }
                return list;
            }
        }
        // For using in IDE debugger.
        internal int DebugComponentsCount
        {
            get
            {
                if (world != null && world.IsAlive() && world.IsEntityAliveInternal(id) && world.GetEntityGen(id) == gen)
                {
                    return world.GetComponentsCount(id);
                }
                return 0;
            }
        }

        // For using in IDE debugger.
        public override string ToString()
        {
            if (id == 0 && gen == 0) { return "Entity-Null"; }
            if (world == null || !world.IsAlive() || !world.IsEntityAliveInternal(id) || world.GetEntityGen(id) != gen) { return "Entity-NonAlive"; }
            System.Type[] types = null;
            var count = world.GetComponentTypes(id, ref types);
            System.Text.StringBuilder sb = null;
            if (count > 0)
            {
                sb = new System.Text.StringBuilder(512);
                for (var i = 0; i < count; i++)
                {
                    if (sb.Length > 0) { sb.Append(","); }
                    sb.Append(types[i].Name);
                }
            }
            return $"Entity-{id}:{gen} [{sb}]";
        }
#endif
    }

#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
    public static partial class EcsEntityExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsPackedEntity PackEntity(this EcsWorld world, int entity)
        {
            return new(entity, world.GetEntityGen(entity));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Unpack(this in EcsPackedEntity packed, EcsWorld world, out int entity)
        {
            if (!world.IsAlive() || !world.IsEntityAliveInternal(packed.id) || world.GetEntityGen(packed.id) != packed.gen)
            {
                entity = -1;
                return false;
            }
            entity = packed.id;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsTo(this in EcsPackedEntity a, in EcsPackedEntity b)
        {
            return a.id == b.id && a.gen == b.gen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsPackedEntityWithWorld PackEntityWithWorld(this EcsWorld world, int entity)
        {
            return new(entity, world.GetEntityGen(entity), world);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Unpack(this in EcsPackedEntityWithWorld packedEntity, out EcsWorld world, out int entity)
        {
            if (packedEntity.world == null || !packedEntity.world.IsAlive() || !packedEntity.world.IsEntityAliveInternal(packedEntity.id) || packedEntity.world.GetEntityGen(packedEntity.id) != packedEntity.gen)
            {
                world = null;
                entity = -1;
                return false;
            }
            world = packedEntity.world;
            entity = packedEntity.id;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsTo(this in EcsPackedEntityWithWorld a, in EcsPackedEntityWithWorld b)
        {
            return a.id == b.id && a.gen == b.gen && a.world == b.world;
        }
    }
}