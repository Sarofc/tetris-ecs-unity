using Saro.Entities;

namespace Tetris
{
    public struct ParentComponent : IEcsComponent
    {
        public EcsEntity parent;

        public override string ToString()
        {
            return $"{nameof(ParentComponent)} {parent}";
        }
    }
}