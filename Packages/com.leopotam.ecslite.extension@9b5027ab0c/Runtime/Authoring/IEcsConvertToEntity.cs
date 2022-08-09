using System;
using Saro;
using UnityEngine;

namespace Leopotam.EcsLite.Authoring
{
    // TODO 这个貌似不太容易支持 entity 层级化
    // 可能还是得搞个 authoring 的东西，来序列化，充分利用monobehaviour
    // authoring 以entity为单位？这样方便嵌套entity？

    // 需要想办法将层级关系自动化

    // 这个就可以当作是 prefab 来用了
    // 数据表的话，可以考虑 生成代码，一个api搞定entity数据赋值。复杂的赋值，依然是手动写代码

    // 补充，华佗也支持直接挂载脚本，所以这个应该不是啥问题了

    public interface IEcsConvertToEntity
    {
        int ConvertToEntity(EcsWorld world);
    }

    public abstract class MonoEntityAuthoring : MonoBehaviour, IEcsConvertToEntity
    {
        public abstract int ConvertToEntity(EcsWorld world);
    }

    public abstract class SOEntityAuthoring : ScriptableObject, IEcsConvertToEntity
    {
        public abstract int ConvertToEntity(EcsWorld world);
    }

    /*
     TODO 这些可能更实用？
    [CreateAssetMenu(menuName = "ECS/GenericSO")]
    public class GenericSOEntityAuthoring : ScriptableObject, IEcsConvertToEntity
    {
        public IEcsComponent[] components = new IEcsComponent[0];

        public int ConvertToEntity(EcsWorld world)
        {
            int ent = world.NewEntity();

            if (components != null && components.Length > 0)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    var module = components[i];
                    var pool = world.GetPoolByType(module.GetType());
                    if (pool != null)
                    {
                        pool.AddRaw(ent, module);
                    }
                    else
                    {
                        Log.ERROR($"please ensure pooltype: {module.GetType()}");
                    }
                }
            }

            PostprocessEntity(world, ent);
            return ent;
        }

        protected virtual void PostprocessEntity(EcsWorld world, int ent)
        {
        }
    }

    public class GenericMonoEntityAuthoring : MonoBehaviour, IEcsConvertToEntity
    {
        public IEcsComponent[] components = new IEcsComponent[0];

        public int ConvertToEntity(EcsWorld world)
        {
            int ent = world.NewEntity();

            if (components != null && components.Length > 0)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    var module = components[i];
                    var pool = world.GetPoolByType(module.GetType());
                    if (pool != null)
                    {
                        pool.AddRaw(ent, module);
                    }
                    else
                    {
                        Log.ERROR($"please ensure pooltype: {module.GetType()}");
                    }
                }
            }

            PostprocessEntity(world, ent);
            return ent;
        }


        protected virtual void PostprocessEntity(EcsWorld world, int ent)
        {
        }
    }
    */
}