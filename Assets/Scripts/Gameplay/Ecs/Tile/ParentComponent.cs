using Leopotam.EcsLite;

namespace Tetris
{
    public struct ParentComponent : IEcsComponent
    {
        public EcsPackedEntity parent;

        public override string ToString()
        {
            return $"{nameof(ParentComponent)} {parent}";
        }
    }
}