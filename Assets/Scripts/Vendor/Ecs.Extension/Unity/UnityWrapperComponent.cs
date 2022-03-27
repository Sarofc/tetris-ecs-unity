
namespace Leopotam.Ecs.Extension
{
    public struct UnityWrapperComponent<T> where T : UnityEngine.Object
    {
        public T value;
    }

    public struct UnityWrapperComponentIgnore<T> where T : UnityEngine.Object, IEcsIgnoreInFilter
    {
        public T value;
    }
}
