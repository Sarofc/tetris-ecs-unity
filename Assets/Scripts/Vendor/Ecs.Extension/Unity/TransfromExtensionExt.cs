using UnityEngine;

namespace Leopotam.Ecs.Extension
{
    public static class TransfromExtensionExt
    {
        public static IEcsMonoProvider GetProvider(this Transform self)
        {
            if (!self.TryGetComponent<IEcsMonoProvider>(out var provider))
            {
                provider = self.gameObject.AddComponent<EcsMonoProvider>();
            }
            return provider;
        }
    }
}
