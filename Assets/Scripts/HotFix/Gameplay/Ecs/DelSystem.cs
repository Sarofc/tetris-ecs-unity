using Leopotam.EcsLite;

namespace Tetris
{
    public sealed class DelSystem<T> : IEcsRunSystem where T : struct, IEcsComponent
    {
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter().Inc<T>().End();
            foreach (var ent in filter) ent.Del<T>(world);
        }
    }

    public static class DelSystemExtension
    {
        public static EcsSystems Del<T>(this EcsSystems systems) where T : struct, IEcsComponent
        {
            systems.Add(new DelSystem<T>());

            return systems;
        }
    }
}