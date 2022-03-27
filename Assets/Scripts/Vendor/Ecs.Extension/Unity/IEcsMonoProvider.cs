using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leopotam.Ecs.Extension
{
    public interface IEcsMonoProvider
    {
        ref EcsEntity Entity { get; }
        bool IsAlive { get; }
        void Link(in EcsEntity ent);
    }
}
