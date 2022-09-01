#if UNITY_EDITOR && !UNITY_WEBPLAYER

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HybridCLR.Editor
{
    public static class AotGenerator
    {
        public static void ScanDLL(Assembly assembly, string checkNs, out IEnumerable<Type> outTypes)
        {
            // TODO 无法获得 方法里 里的调用的 静态方法 的type

            // 目前只收集 类中的字段，属性，方法参数、返回值。

            var set = new HashSet<Type>();
            outTypes = set;

            var types = assembly.GetTypes();

            var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var type in types)
            {
                if (type.IsClass)
                {
                    var fields = type.GetFields(bindingFlags);
                    foreach (var field in fields)
                    {
                        AddToListIfNeeded(field.FieldType, checkNs, set);
                    }

                    var properties = type.GetProperties(bindingFlags);
                    foreach (var property in properties)
                    {
                        AddToListIfNeeded(property.PropertyType, checkNs, set);
                    }

                    var methods = type.GetMethods(bindingFlags);
                    foreach (var method in methods)
                    {
                        AddToListIfNeeded(method.ReturnType, checkNs, set);

                        var parameters = method.GetParameters();
                        foreach (var parameter in parameters)
                        {
                            AddToListIfNeeded(parameter.ParameterType, checkNs, set);
                        }

                        var body = method.GetMethodBody();
                        if (body != null)
                        {
                            foreach (var local in body.LocalVariables)
                            {
                                AddToListIfNeeded(local.LocalType, checkNs, set);
                            }
                        }
                    }
                }
            }
        }

        private static void AddToListIfNeeded(Type type, string checkNs, HashSet<Type> outTypes)
        {
            if (type.IsGenericType)
            {
                foreach (var constraint in type.GetGenericArguments())
                {
                    AddToListIfNeeded(constraint, checkNs, outTypes);
                }
            }
            else
            {
                var ret = __AddToListIfNeeded(type, checkNs, outTypes);
                if (ret) UnityEngine.Debug.LogError($"add {type.ToString()}. isArray: {type.IsArray}");
            }
        }

        private static bool __AddToListIfNeeded(Type type, string checkNs, HashSet<Type> outTypes)
        {
            var ns = type.Namespace;

            if (!string.IsNullOrEmpty(ns))
            {
                if (ns.StartsWith(checkNs))
                {
                    if (string.IsNullOrEmpty(type.FullName))
                    {
                        Debug.LogError($"{type}.FullName IsNullOrEmpty ");
                        return false;
                    }
                    else if (type.HasElementType)
                    {
                        var elementType = type.GetElementType();
                        Debug.LogError($"{type} HasElementType: {elementType}");
                        return outTypes.Add(elementType);
                    }
                    else if (type.IsPointer)
                    {
                        Debug.LogError($"{type} IsPointer");
                        //return false;
                    }
                    else if (type.IsByRef)
                    {
                        Debug.LogError($"{type} IsByRef");
                        // UnityEngine.Vector2& IsByRef
                    }
                    else if (type.IsByRefLike)
                    {
                        Debug.LogError($"{type} IsByRefLike");
                        //return false;
                    }
                    else
                    {
                        return outTypes.Add(type);
                    }
                }
            }

            return false;
        }

        public static void GenerateLinkXML(IEnumerable<Type> types, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var pairs = new Dictionary<string, List<Type>>();
            foreach (var type in types)
            {
                if (type == null)
                {
                    Debug.LogError($"type is null");
                }

                if (type.Assembly == null)
                {
                    Debug.LogError($"{type}'s Assembly is null");
                }

                var asmName = type.Assembly.GetName().Name;
                if (!pairs.ContainsKey(asmName))
                {
                    pairs[asmName] = new List<Type>();
                }
                pairs[asmName].Add(type);
            }

            var sb = new StringBuilder();

            sb.AppendLine("<linker>");

            foreach (var pair in pairs)
            {
                sb.AppendLine(string.Format("	<assembly fullname=\"{0}\">", pair.Key));

                foreach (var type in pair.Value)
                {
                    sb.AppendLine("		<type fullname=\"" + type.FullName + "\" preserve=\"all\"/>");
                }

                sb.AppendLine("	</assembly>");
            }

            sb.AppendLine("</linker>");
            File.WriteAllText(path, sb.ToString());

            Debug.LogError($"GenerateLinkXML to {path}");
        }
    }
}

#endif