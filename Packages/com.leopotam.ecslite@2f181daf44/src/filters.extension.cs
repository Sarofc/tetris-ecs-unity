namespace Leopotam.EcsLite
{
    public sealed partial class EcsFilter
    {
        public int this[int index] => _denseEntities[index];

        public int EntitiesCount => _entitiesCount;
    }
}