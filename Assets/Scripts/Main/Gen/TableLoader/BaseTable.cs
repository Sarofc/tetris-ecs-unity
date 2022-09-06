using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Saro.Table
{
    public abstract class BaseTable<TValue, TWrapper> where TWrapper : new()
    {

        private static readonly object s_Lock = new object();
        public static TWrapper Get()
        {
            lock (s_Lock)
            {
                if (s_Instance == null)
                {
                    s_Instance = new TWrapper();
                    (s_Instance as BaseTable<TValue, TWrapper>).m_Datas = new Dictionary<ulong, TValue>();
                }
            }

            return s_Instance;
        }

        protected static TWrapper s_Instance;

        protected Dictionary<ulong, TValue> m_Datas;

        protected bool m_Loaded;

        public Dictionary<ulong, TValue> GetTable()
        {
            return m_Datas;
        }

        public abstract bool Load();

        public abstract ValueTask<bool> LoadAsync();

        protected byte[] GetBytes(string tableName)
        {
            if (TableLoader.s_BytesLoader == null)
            {
                throw new NullReferenceException("MUST register TableLoader.s_BytesLoader handler");
            }

            return TableLoader.s_BytesLoader(tableName);
        }

        protected async ValueTask<byte[]> GetBytesAsync(string tableName)
        {
            if (TableLoader.s_BytesLoaderAsync == null)
            {
                throw new NullReferenceException("MUST register TableLoader.s_BytesLoaderAsync handler");
            }

            return await TableLoader.s_BytesLoaderAsync(tableName);
        }

        public void Unload()
        {
            m_Loaded = false;
            m_Datas = null;
            s_Instance = default;
        }
    }
}