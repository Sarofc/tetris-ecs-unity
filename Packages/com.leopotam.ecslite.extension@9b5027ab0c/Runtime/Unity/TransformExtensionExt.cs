using UnityEngine;

namespace Leopotam.EcsLite.Extension
{
    public static class TransformExtensionExt
    {
        public static IEcsMonoLink Link(this Transform self)
        {
            if (!self.TryGetComponent<IEcsMonoLink>(out var link))
            {
                link = self.gameObject.AddComponent<EcsMonoLink>();
            }
            return link;
        }
    }
}
