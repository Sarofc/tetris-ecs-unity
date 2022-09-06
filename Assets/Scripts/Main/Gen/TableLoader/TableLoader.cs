using System;
using System.Threading.Tasks;

namespace Saro.Table
{
    public static class TableLoader
    {
        /// <summary>
        /// 数据表版本
        /// </summary>
        public const int k_DataVersion = 4;

        /// <summary>
        /// 数据表加载委托
        /// <code>string: 表名</code>
        /// <code>byte[]: 表二进制数据</code>
        /// </summary>
        public static Func<string, byte[]> s_BytesLoader;

        /// <summary>
        /// 数据表加载委托
        /// <code>string: 表名</code>
        /// <code>byte[]: 表二进制数据</code>
        /// </summary>
        public static Func<string, ValueTask<byte[]>> s_BytesLoaderAsync;
    }
}
