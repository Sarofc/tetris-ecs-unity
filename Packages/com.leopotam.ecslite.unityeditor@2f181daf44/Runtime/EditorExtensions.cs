// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

namespace Leopotam.EcsLite.UnityEditor
{
    public static class EditorExtensions
    {
        public static string GetCleanGenericTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }
            var constraints = "";
            foreach (var constraint in type.GetGenericArguments())
            {
                constraints += constraints.Length > 0 ? $", {GetCleanGenericTypeName(constraint)}" : constraint.Name;
            }
            return $"{type.Name.Substring(0, type.Name.LastIndexOf("`", StringComparison.Ordinal))}<{constraints}>";
        }
    }

    public sealed class EcsEntityDebugView : MonoBehaviour
    {
        [NonSerialized]
        public EcsWorld world;
        [NonSerialized]
        public int entity;
        [NonSerialized]
        public EcsWorldDebugSystem debugSystem;
    }

    public sealed class EcsSystemsDebugView : MonoBehaviour
    {
        [NonSerialized]
        public List<EcsSystems> ecsSystemsList;
        public EcsWorldDebugSystem debugSystem;
    }

    public sealed class EcsWorldDebugView : MonoBehaviour
    {
        [NonSerialized]
        public EcsWorld ecsWorld;
        public EcsWorldDebugSystem debugSystem;
    }
}
#endif