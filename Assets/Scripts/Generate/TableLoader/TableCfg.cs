using System;

namespace Saro.Table
{
    public sealed class TableCfg
    {
        /// <summary>
        /// 数据表版本
        /// </summary>
        public const int k_DataVersion = 3;

        /// <summary>
        /// 数据表加载委托
        /// <code>string: 表名</code>
        /// <code>byte[]: 表二进制数据</code>
        /// </summary>
        public static Func<string, byte[]> s_BytesLoader;
    }
}
