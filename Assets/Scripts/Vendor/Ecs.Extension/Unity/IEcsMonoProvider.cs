namespace Leopotam.Ecs.Extension
{
    public interface IEcsMonoProvider
    {
        ref EcsEntity Entity { get; }
        bool IsAlive { get; }
        void Link(in EcsEntity ent);
    }
}
