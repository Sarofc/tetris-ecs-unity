
using Leopotam.Ecs;

namespace Tetris
{
    public struct ParentComponent
    {
        public EcsEntity parent;

        public override string ToString()
        {
            return $"{nameof(ParentComponent)} {parent}";
        }
    }
}
