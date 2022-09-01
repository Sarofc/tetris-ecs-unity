using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HybridCLR.Test
{
    internal class TestSpan
    {
        public static void DoSpanTest<T>(StringBuilder builder, T value = default) where T : unmanaged, IEquatable<T>
        {
            try
            {
                Span<T> span = stackalloc T[10];
                span[2] = value;

                bool equal = span[2].Equals(value);
                if (!equal)
                {
                    builder.AppendLine($"   [error] {span[2]} != {value}");
                }
            }
            catch (Exception e)
            {
                var lines = e.StackTrace.Split('\n');
                builder.AppendLine($"   [exception] Span<{typeof(T)}> span = stackalloc {typeof(T)}[10]; {e}");
            }
        }
    }
}
