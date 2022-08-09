// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

namespace Leopotam.EcsLite
{
    [System.Obsolete("TODO 性能太低", true)]
    public static partial class EcsEntityExtensions
    {
        /*
            var pool = ent.World.GetPool<T>(); 大数组时，非常耗时

            改为 ComponentPool.TPool.直接引用 EcsPool
         */

        // TODO 没有检查 ent 是否存活
        public static ref T Add<T>(this EcsPackedEntity ent, EcsWorld world) where T : struct, IEcsComponent
        {
            var pool = world.GetPool<T>();
            return ref pool.Add(ent.id);
        }
        
        public static ref T Add<T>(this EcsPackedEntityWithWorld ent) where T : struct, IEcsComponent
        {
            var pool = ent.world.GetPool<T>();
            return ref pool.Add(ent.id);
        }

        // TODO 没有检查 ent 是否存活
        public static ref T Add<T>(this int ent, EcsWorld world) where T : struct, IEcsComponent
        {
            var pool = world.GetPool<T>();
            return ref pool.Add(ent);
        }

        public static ref T Add<T>(this int ent, T component, EcsWorld world) where T : struct, IEcsComponent
        {
            var pool = world.GetPool<T>();
            ref var cc = ref pool.Add(ent);
            cc = component;
            return ref cc;
        }

        // TODO 没有检查 ent 是否存活
        public static bool Has<T>(this int ent, EcsWorld world) where T : struct, IEcsComponent
        {
            var pool = world.GetPool<T>();
            return pool.Has(ent);
        }
        
        public static bool Has<T>(this EcsPackedEntity ent, EcsWorld world) where T : struct, IEcsComponent
        {
            var pool = world.GetPool<T>();
            return pool.Has(ent.id);
        }

        // TODO 没有检查 ent 是否存活
        public static bool Has<T>(this EcsPackedEntityWithWorld ent) where T : struct, IEcsComponent
        {
            var pool = ent.world.GetPool<T>();
            return pool.Has(ent.id);
        }

        // TODO 没有检查 ent 是否存活
        public static ref T Get<T>(this EcsPackedEntityWithWorld self) where T : struct, IEcsComponent
        {
            var pool = self.world.GetPool<T>();
            return ref pool.Get(self.id);
        }

        // TODO 没有检查 ent 是否存活
        public static ref T Get<T>(this EcsPackedEntity self, EcsWorld world) where T : struct, IEcsComponent
        {
            //if(self.Unpack(world, out _))
            {
                var pool = world.GetPool<T>();
                return ref pool.Get(self.id);
            }
        }

        // TODO 没有检查 ent 是否存活
        public static ref T Get<T>(this int ent, EcsWorld world) where T : struct, IEcsComponent
        {
            var pool = world.GetPool<T>();
            return ref pool.Get(ent);
        }

        // TODO 没有检查 ent 是否存活
        public static void Del<T>(this EcsPackedEntityWithWorld self) where T : struct, IEcsComponent
        {
            var pool = self.world.GetPool<T>();
            pool.Del(self.id);
        }
        
        // TODO 没有检查 ent 是否存活
        public static void Del<T>(this EcsPackedEntity self, EcsWorld world) where T : struct, IEcsComponent
        {
            var pool = world.GetPool<T>();
            pool.Del(self.id);
        }

        // TODO 没有检查 ent 是否存活
        public static void Del<T>(this int ent, EcsWorld world) where T : struct, IEcsComponent
        {
            var pool = world.GetPool<T>();
            pool.Del(ent);
        }

        public static bool IsNull(this EcsPackedEntity self) => self.id == 0 && self.gen == 0;
        public static bool IsAlive(this EcsPackedEntity self, EcsWorld world) => self.Unpack(world, out _);
        //public static void Destroy(this EcsPackedEntity self, EcsWorld world) => world.DelEntity(self.Id);

        public static bool IsNull(this EcsPackedEntityWithWorld self) => self.id == 0 && self.gen == 0;
        public static bool IsAlive(this EcsPackedEntityWithWorld self) => self.Unpack(out _, out _);
        public static void Destroy(this EcsPackedEntityWithWorld self) => self.world.DelEntity(self.id);
        //public static void Destroy(this int self, EcsWorld world) => world.DelEntity(self);
    }
}