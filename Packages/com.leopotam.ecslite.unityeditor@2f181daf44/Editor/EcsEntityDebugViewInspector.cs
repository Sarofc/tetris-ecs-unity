// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Leopotam.EcsLite.UnityEditor
{
    [CustomEditor(typeof(EcsEntityDebugView))]
    internal sealed class EcsEntityDebugViewInspector : Editor
    {
        private const int MaxFieldToStringLength = 128;
        private static object[] _componentsCache = new object[32];

        public override void OnInspectorGUI()
        {
            var observer = (EcsEntityDebugView) target;
            if (observer.world != null)
            {
                bool guiEnable = GUI.enabled;
                GUI.enabled = observer.world.IsEntityAliveInternal(observer.entity);
                if (GUILayout.Button("Destroy"))
                {
                    observer.world.DelEntity(observer.entity);
                }
                GUI.enabled = guiEnable;

                DrawComponents(observer);
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawComponents(EcsEntityDebugView debugView)
        {
            if (debugView.gameObject.activeSelf)
            {
                var count = debugView.world.GetComponents(debugView.entity, ref _componentsCache);
                for (var i = 0; i < count; i++)
                {
                    var component = _componentsCache[i];
                    _componentsCache[i] = null;
                    var type = component.GetType();
                    GUILayout.BeginVertical("helpbox");
                    var typeName = EditorExtensions.GetCleanGenericTypeName(type);
                    var pool = debugView.world.GetPoolByType(type);
                    var (rendered, changed, newValue) = EcsComponentInspectors.Render(typeName, type, component, debugView);
                    if (!rendered)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(typeName, EditorStyles.boldLabel);
                        if (GUILayout.Button("-", GUILayout.Width(24)))
                        {
                            pool.Del(debugView.entity);
                        }
                        EditorGUILayout.EndHorizontal();
                        var indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel++;
                        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                        {
                            DrawTypeField(component, pool, field, debugView);
                        }
                        EditorGUI.indentLevel = indent;
                    }
                    else
                    {
                        if (changed)
                        {
                            // update value.
                            pool.SetRaw(debugView.entity, newValue);
                        }
                    }

                    GUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }
        }

        private void DrawTypeField(object component, IEcsPool pool, FieldInfo field, EcsEntityDebugView debugView)
        {
            var fieldValue = field.GetValue(component);
            var fieldType = field.FieldType;
            var (rendered, changed, newValue) = EcsComponentInspectors.Render(field.Name, fieldType, fieldValue, debugView);
            if (!rendered)
            {
                if (fieldType == typeof(Object) || fieldType.IsSubclassOf(typeof(Object)))
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name, GUILayout.MaxWidth(EditorGUIUtility.labelWidth - 16));
                    var newObjValue = EditorGUILayout.ObjectField(fieldValue as Object, fieldType, true);
                    if (newObjValue != (Object) fieldValue)
                    {
                        field.SetValue(component, newObjValue);
                        pool.SetRaw(debugView.entity, component);
                    }

                    GUILayout.EndHorizontal();
                }
                else if (fieldType.IsEnum)
                {
                    var isFlags = Attribute.IsDefined(fieldType, typeof(FlagsAttribute));
                    var (enumChanged, enumNewValue) = EcsComponentInspectors.RenderEnum(field.Name, fieldValue, isFlags);
                    if (enumChanged)
                    {
                        field.SetValue(component, enumNewValue);
                        pool.SetRaw(debugView.entity, component);
                    }
                }
                else
                {
                    var strVal = fieldValue != null ? string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", fieldValue) : "null";
                    if (strVal.Length > MaxFieldToStringLength)
                    {
                        strVal = strVal.Substring(0, MaxFieldToStringLength);
                    }

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(field.Name);
                    EditorGUILayout.SelectableLabel(strVal, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                if (changed)
                {
                    // update value.
                    field.SetValue(component, newValue);
                    pool.SetRaw(debugView.entity, component);
                }
            }
        }
    }

    internal static class EcsComponentInspectors
    {
        private static readonly Dictionary<Type, IEcsComponentInspector> Inspectors = new Dictionary<Type, IEcsComponentInspector>();

        static EcsComponentInspectors()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IEcsComponentInspector).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        if (Activator.CreateInstance(type) is IEcsComponentInspector inspector)
                        {
                            var componentType = inspector.GetFieldType();
                            if (!Inspectors.TryGetValue(componentType, out var prevInspector)
                                || inspector.GetPriority() > prevInspector.GetPriority())
                            {
                                Inspectors[componentType] = inspector;
                            }
                        }
                    }
                }
            }
        }

        public static (bool, bool, object) Render(string label, Type type, object value, EcsEntityDebugView debugView)
        {
            if (Inspectors.TryGetValue(type, out var inspector))
            {
                var (changed, newValue) = inspector.OnGui(label, value, debugView);
                return (true, changed, newValue);
            }

            return (false, false, null);
        }

        public static (bool, object) RenderEnum(string label, object value, bool isFlags)
        {
            var enumValue = (Enum) value;
            Enum newValue;
            if (isFlags)
            {
                newValue = EditorGUILayout.EnumFlagsField(label, enumValue);
            }
            else
            {
                newValue = EditorGUILayout.EnumPopup(label, enumValue);
            }

            if (Equals(newValue, value))
            {
                return (default, default);
            }

            return (true, newValue);
        }
    }

    public interface IEcsComponentInspector
    {
        Type GetFieldType();
        int GetPriority();
        (bool, object) OnGui(string label, object value, EcsEntityDebugView entityView);
    }

    public abstract class EcsComponentInspectorTyped<T> : IEcsComponentInspector
    {
        public Type GetFieldType() => typeof(T);
        protected virtual bool IsNullAllowed() => false;
        public virtual int GetPriority() => 0;

        (bool, object) IEcsComponentInspector.OnGui(string label, object value, EcsEntityDebugView entityView)
        {
            if (value == null && !IsNullAllowed())
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(label);
                EditorGUILayout.SelectableLabel("null", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
                return (default, default);
            }

            var typedValue = (T) value;
            var changed = OnGuiTyped(label, ref typedValue, entityView);
            if (changed)
            {
                return (true, typedValue);
            }

            return (default, default);
        }

        protected abstract bool OnGuiTyped(string label, ref T value, EcsEntityDebugView entityView);
    }
}