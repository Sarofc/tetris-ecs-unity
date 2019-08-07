using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saro {

    /// <summary>
    /// C# object pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> where T : new () {
        private Stack<T> m_Objects = new Stack<T> ();
        private bool m_FixedSize;
        private int m_PoolSize;

        public ObjectPool (int poolSize = 10, bool fixedSize = true) {

            this.m_PoolSize = poolSize;
            this.m_FixedSize = fixedSize;

            for (int i = 0; i < poolSize; i++) {
                m_Objects.Push (new T ());
            }
        }

        public T New () {
            T item = default (T);
            if (m_Objects.Count > 0) {
                item = m_Objects.Pop ();
            } else {

                if (!m_FixedSize) {
                    m_PoolSize++;
                } else {
                    Debug.LogWarning ("No object avaliable,Create new Instance!");
                }

                item = new T ();
            }
            return item;
        }

        public void Free (T item) {
            if (!m_FixedSize || m_PoolSize > m_Objects.Count) {
                m_Objects.Push (item);
            } else {
                Debug.LogWarning ("Instance can't return to Pool,wait GC");
            }
        }
    }

    /// <summary>
    /// unity object pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> where T : MonoBehaviour, IPoolable<T> {
        private Stack<T> m_Objects = new Stack<T> ();
        private bool m_FixedSize;
        private int m_PoolSize;

        private T m_Instance;
        //Callback init or repool
        private Action<T> m_OnPooled;

        /// <summary>
        /// constructor of pool
        /// </summary>
        /// <param name="instance">object instance</param>
        /// <param name="onPooled">callback</param>
        /// <param name="poolSize">pool size</param>
        /// <param name="fixedSize">fixed size</param>
        public Pool (T instance, Action<T> onPooled, int poolSize = 10, bool fixedSize = true) {
            this.m_PoolSize = poolSize;
            this.m_FixedSize = fixedSize;
            this.m_Instance = instance;

            this.m_OnPooled = onPooled;

            for (int i = 0; i < poolSize; i++) {
                T item = GameObject.Instantiate<T> (m_Instance);
                m_Objects.Push (item);

                item.Pool = this;

                m_OnPooled?.Invoke (item);

                item.InPooled = true;
            }
        }

        public T New () {
            T item = default (T);

            if (m_Objects.Count > 0) {
                item = m_Objects.Pop ();
            } else {
                if (!m_FixedSize) {
                    m_PoolSize++;
                } else {
                    Debug.LogWarning ("No object avaliable,Create new Instance!");
                }

                item = GameObject.Instantiate<T> (m_Instance);
                item.Pool = this;
            }

            if (item) {
                item.InPooled = false;
            }
            return item;
        }

        public void Free (T item) {

            if (!item) {
                Debug.LogError ("Null refrence");
                return;
            }

            if (item.Pool == this && !item.InPooled) {
                if (!m_FixedSize || m_PoolSize > m_Objects.Count) {

                    m_Objects.Push (item);

                    m_OnPooled?.Invoke (item);

                    item.InPooled = true;
                } else {
                    GameObject.Destroy (item.gameObject);
                    Debug.LogWarning ("Instance can't return to Pool,destroy Instance");
                }
            }
        }

        // 释放资源
        public void ClearPool () {
            foreach (var item in m_Objects) {
                GameObject.Destroy (item);
            }
            m_Objects.Clear ();
            m_OnPooled = null;
        }
    }

    public interface IPoolable<T> where T : MonoBehaviour, IPoolable<T> {
        /// <summary>
        /// aready in pool？
        /// </summary>
        /// <value></value>
        bool InPooled { get; set; }
        /// <summary>
        ///whitch pool does this object belong to
        /// </summary>
        /// <value></value>
        Pool<T> Pool { get; set; }
    }
}