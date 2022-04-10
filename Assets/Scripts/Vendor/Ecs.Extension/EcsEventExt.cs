namespace Leopotam.Ecs.Extension
{
    public static class EcsEventExt
    {
        public static void SendMessage<T>(this EcsWorld self, in T @event) where T : struct
        {
            self.NewEntity().Replace(@event);
            //Log.ERROR($"SendMessage: {@event}");
        }

        public static void SendMessage<T1, T2>(this EcsWorld self, in T1 evt1, in T2 evt2) where T1 : struct where T2 : struct
        {
            self.NewEntity().Replace(evt1).Replace(evt2);
            //Log.ERROR($"SendMessage: {evt1} {evt2}");
        }
    }
}
