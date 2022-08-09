// #define LOG_ECS_MSG

using Saro;

namespace Leopotam.EcsLite.Extension
{
    public static class EcsEventExt
    {
        public static void SendMessage<T>(this EcsWorld self, in T e) where T : struct, IEcsComponent
        {
            var ent = self.NewEntity();

            {
                ref var msg = ref ent.Add<T>(self);
                msg = e;
            }

#if LOG_ECS_MSG
            Log.ERROR($"SendMessage: {e}");
#endif
        }

        public static void SendMessage<T1, T2>(this EcsWorld self, in T1 e1, in T2 e2) where T1 : struct, IEcsComponent where T2 : struct, IEcsComponent
        {
            var ent = self.NewEntity();

            {
                ref var msg = ref ent.Add<T1>(self);
                msg = e1;
            }

            {
                ref var msg = ref ent.Add<T2>(self);
                msg = e2;
            }

#if LOG_ECS_MSG
            Log.ERROR($"SendMessage: {e1} {e2}");
#endif
        }
    }
}
