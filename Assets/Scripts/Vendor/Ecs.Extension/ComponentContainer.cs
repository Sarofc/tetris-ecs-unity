using Saro.Pool;
using System.Collections.Generic;

namespace Leopotam.Ecs.Extension
{
    /// <summary>
    /// TODO 类似 unity dynamic buffer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ComponentArray<T> : IEcsAutoReset<ComponentArray<T>>
        where T : struct
    {
        public T[] Value;

        public void AutoReset(ref ComponentArray<T> c)
        {
        }
    }

    public struct ComponentList<T> : IEcsAutoReset<ComponentList<T>>
        where T : struct
    {
        public List<T> Value => m_Value ??= ListPool<T>.Rent();
        private List<T> m_Value;

        public void AutoReset(ref ComponentList<T> c)
        {
            if (c.m_Value != null)
            {
                ListPool<T>.Return(c.m_Value);
                c.m_Value = null;
            }
        }
    }

    public struct ComponentStack<T> where T : struct
    {
        public Stack<T> Value => m_Value ??= new Stack<T>();
        private Stack<T> m_Value;
    }

    public struct ComponentQueue<T> where T : struct
    {
        public Queue<T> Value => m_Value ??= new Queue<T>();
        private Queue<T> m_Value;
    }
}
