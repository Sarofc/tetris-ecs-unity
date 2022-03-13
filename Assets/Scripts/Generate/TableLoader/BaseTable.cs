using System;
using System.Collections.Generic;
using System.IO;

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

        protected byte[] GetBytes(string tableName)
        {
            if (TableCfg.s_BytesLoader != null)
            {
                //var path = Path.Combine(TableCfg.s_TableSrc, tableName);
                return TableCfg.s_BytesLoader(tableName);
            }
            return null;
        }

        public void Unload()
        {
            m_Loaded = false;
            m_Datas = null;
            s_Instance = default;
        }
    }
}