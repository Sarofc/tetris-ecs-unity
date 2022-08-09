namespace Leopotam.EcsLite
{
    public partial class EcsWorld
    {
        private int m_SingletonEntityId = -1;

        public int GetSingletonEntity()
        {
            if (m_SingletonEntityId < 0)
                m_SingletonEntityId = NewEntity();

            return m_SingletonEntityId;
        }

        public ref T GetSingleton<T>() where T : struct, IEcsComponent
        {
            var singletonID = GetSingletonEntity();

            return ref GetPool<T>(0, 1).Add(singletonID);
        }
    }
}