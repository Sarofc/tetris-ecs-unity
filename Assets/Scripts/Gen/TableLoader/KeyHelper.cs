using System;
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
            return ((ulong)key1 & 0xffffffff) | (((ulong)key2 & 0xffffffff) << 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKey(int key1, int key2, int key3)
        {
            var shortKey2 = Convert.ToInt16(key2);
            var shortKey3 = Convert.ToInt16(key3);
            return ((ulong)key1 & 0xffffffff) | (((ulong)shortKey2 & 0xffff) << 32) |
                   (((ulong)shortKey3 & 0xffff) << 48);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKey(int key1, int key2, int key3, int key4)
        {
            var shortKey1 = Convert.ToInt16(key1);
            var shortKey2 = Convert.ToInt16(key2);
            var shortKey3 = Convert.ToInt16(key3);
            var shortKey4 = Convert.ToInt16(key4);
            return ((ulong)shortKey1 & 0xffff) | (((ulong)shortKey2 & 0xffff) << 16) |
                   (((ulong)shortKey3 & 0xffff) << 32) | (((ulong)shortKey4 & 0xffff) << 48);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int key1, int key2) SplitKey2(ulong key)
        {
            var unsignedKey = key;
            var key1 = (uint)(unsignedKey & 0xffffffffUL);
            var key2 = (uint)(unsignedKey >> 32);
            var i1 = (int)key1;
            var i2 = (int)key2;

            return (i1, i2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int key1, int key2, int key3) SplitKey3(ulong key)
        {
            var unsignedKey = key;
            var key1 = (int)(unsignedKey & 0xffffffffUL);
            var key3 = (int)(unsignedKey >> 48);
            var key2 = (int)(unsignedKey >> 32) - key3;

            return (key1, key2, key3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int key1, int key2, int key3, int key4) SplitKey4(ulong key)
        {
            var unsignedKey = key;
            var key1 = (uint)(unsignedKey & 0xffff);
            var key2 = ((uint)unsignedKey >> 16) | 0xffff;
            var key3 = ((uint)unsignedKey >> 32) | 0xffff;
            var key4 = ((uint)unsignedKey >> 48) | 0xffff;
            var i1 = (int)key1;
            var i2 = (int)key2;
            var i3 = (int)key3;
            var i4 = (int)key4;

            return (i1, i2, i3, i4);
        }
    }
}