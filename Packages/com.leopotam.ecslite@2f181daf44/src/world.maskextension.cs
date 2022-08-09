
using System.Runtime.CompilerServices;

namespace Leopotam.EcsLite
{
    public partial class EcsWorld
    {
        public sealed partial class Mask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Inc<T1, T2>()
                where T1 : struct, IEcsComponent
                where T2 : struct, IEcsComponent
            {
                return Inc<T1>().Inc<T2>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Inc<T1, T2, T3>()
                where T1 : struct, IEcsComponent
                where T2 : struct, IEcsComponent
                where T3 : struct, IEcsComponent
            {
                return Inc<T1>().Inc<T2>().Inc<T3>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Inc<T1, T2, T3, T4>()
               where T1 : struct, IEcsComponent
               where T2 : struct, IEcsComponent
               where T3 : struct, IEcsComponent
               where T4 : struct, IEcsComponent
            {
                return Inc<T1>().Inc<T2>().Inc<T3>().Inc<T4>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Inc<T1, T2, T3, T4, T5>()
               where T1 : struct, IEcsComponent
               where T2 : struct, IEcsComponent
               where T3 : struct, IEcsComponent
               where T4 : struct, IEcsComponent
               where T5 : struct, IEcsComponent
            {
                return Inc<T1>().Inc<T2>().Inc<T3>().Inc<T4>().Inc<T5>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Exc<T1, T2>()
               where T1 : struct, IEcsComponent
               where T2 : struct, IEcsComponent
            {
                return Exc<T1>().Exc<T2>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Exc<T1, T2, T3>()
                where T1 : struct, IEcsComponent
                where T2 : struct, IEcsComponent
                where T3 : struct, IEcsComponent
            {
                return Exc<T1>().Exc<T2>().Exc<T3>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Exc<T1, T2, T3, T4>()
               where T1 : struct, IEcsComponent
               where T2 : struct, IEcsComponent
               where T3 : struct, IEcsComponent
               where T4 : struct, IEcsComponent
            {
                return Exc<T1>().Exc<T2>().Exc<T3>().Exc<T4>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mask Exc<T1, T2, T3, T4, T5>()
               where T1 : struct, IEcsComponent
               where T2 : struct, IEcsComponent
               where T3 : struct, IEcsComponent
               where T4 : struct, IEcsComponent
               where T5 : struct, IEcsComponent
            {
                return Exc<T1>().Exc<T2>().Exc<T3>().Exc<T4>().Exc<T5>();
            }
        }
    }
}
