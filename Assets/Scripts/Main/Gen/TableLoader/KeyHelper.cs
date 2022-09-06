
using System.Runtime.CompilerServices;

namespace Saro.Table
{
    public class KeyHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKey(int key1)
        {
            return (ulong)key1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKey(int key1, int key2)
        {
            return (((ulong)key1 & 0xffffffff) | (((ulong)key2 & 0xffffffff) << 32));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKey(int key1, int key2, int key3)
        {
            short shortKey2 = System.Convert.ToInt16(key2);
            short shortKey3 = System.Convert.ToInt16(key3);
            return (((ulong)key1 & 0xffffffff) | (((ulong)shortKey2 & 0xffff) << 32) | (((ulong)shortKey3 & 0xffff) << 48));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKey(int key1, int key2, int key3, int key4)
        {
            short shortKey1 = System.Convert.ToInt16(key1);
            short shortKey2 = System.Convert.ToInt16(key2);
            short shortKey3 = System.Convert.ToInt16(key3);
            short shortKey4 = System.Convert.ToInt16(key4);
            return (((ulong)shortKey1 & 0xffff) | (((ulong)shortKey2 & 0xffff) << 16) | (((ulong)shortKey3 & 0xffff) << 32) | (((ulong)shortKey4 & 0xffff) << 48));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int key1, int key2) SplitKey2(ulong key)
        {
            ulong unsignedKey = key;
            uint _key1 = (uint)(unsignedKey & 0xffffffffUL);
            uint _key2 = (uint)(unsignedKey >> 32);
            int i1 = (int)_key1;
            int i2 = (int)_key2;

            return (i1, i2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int key1, int key2, int key3) SplitKey3(ulong key)
        {
            ulong unsignedKey = key;
            int _key1 = (int)(unsignedKey & 0xffffffffUL);
            int _key3 = (int)(unsignedKey >> 48);
            int _key2 = (int)(unsignedKey >> 32) - _key3;

            return (_key1, _key2, _key3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int key1, int key2, int key3, int key4) SplitKey4(ulong key)
        {
            ulong unsignedKey = key;
            uint _key1 = (uint)(unsignedKey & 0xffff);
            uint _key2 = (uint)(unsignedKey) >> 16 | 0xffff;
            uint _key3 = (uint)(unsignedKey) >> 32 | 0xffff;
            uint _key4 = (uint)(unsignedKey) >> 48 | 0xffff;
            int i1 = (int)_key1;
            int i2 = (int)_key2;
            int i3 = (int)_key3;
            int i4 = (int)_key4;

            return (i1, i2, i3, i4);
        }
    }
}
