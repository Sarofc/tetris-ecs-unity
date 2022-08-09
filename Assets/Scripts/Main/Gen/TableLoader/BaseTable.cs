using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saro.Table
{
    public abstract class BaseTable<TValue, TWrapper> where TWrapper : new()
    {
        private static readonly object s_Lock = new();

        protected static TWrapper instance;

        protected Dictionary<ulong, TValue> datas;

        protected bool loaded;

        public static TWrapper Get()
        {
            lock (s_Lock)
            {
                if (instance == null)
                {
                    instance = new TWrapper();
                    (instance as BaseTable<TValue, TWrapper>).datas = new Dictionary<ulong, TValue>();
                }
            }

            return instance;
        }

        public Dictionary<ulong, TValue> GetTable()
        {
            return datas;
        }

        public abstract bool Load();

        public abstract ValueTask<bool> LoadAsync();

        protected byte[] GetBytes(string tableName)
        {
            if (TableLoader.bytesLoader != null) return TableLoader.bytesLoader(tableName);
            return null;
        }

        protected async ValueTask<byte[]> GetBytesAsync(string tableName)
        {
            if (TableLoader.bytesLoaderAsync != null) return await TableLoader.bytesLoaderAsync(tableName);
            return null;
        }

        public void Unload()
        {
            loaded = false;
            datas = null;
            instance = default;
        }
    }
}