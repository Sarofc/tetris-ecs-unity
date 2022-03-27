// ----------------------------------------------------------------------------
// The MIT License
// Unity integration https://github.com/Leopotam/ecs-unityintegration
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2021 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Leopotam.Ecs.UnityIntegration.Editor {
    [CustomEditor (typeof (EcsEntityObserver))]
    sealed class EcsEntityObserverInspector : UnityEditor.Editor {
        const int MaxFieldToStringLength = 128;

        static object[] _componentsCache = new object[32];

        EcsEntityObserver _observer;

        public override void OnInspectorGUI () {
            if (_observer.World != null) {
                var guiEnabled = GUI.enabled;
                GUI.enabled = true;
                DrawComponents ();
                GUI.enabled = guiEnabled;
                EditorUtility.SetDirty (target);
            }
        }

        void OnEnable () {
            _observer = target as EcsEntityObserver;
        }

        void OnDisable () {
            _observer = null;
        }

        void DrawComponents () {
            if (_observer.gameObject.activeSelf) {
                var count = _observer.Entity.IsAlive () ? _observer.Entity.GetComponentValues (ref _componentsCache) : 0;
                for (var i = 0; i < count; i++) {
                    var component = _componentsCache[i];
                    _componentsCache[i] = null;
                    var type = component.GetType ();
                    GUILayout.BeginVertical (GUI.skin.box);
                    var typeName = EditorHelpers.GetCleanGenericTypeName (type);
                    if (!EcsComponentInspectors.Render (typeName, type, component, _observer)) {
                        EditorGUILayout.LabelField (typeName, EditorStyles.boldLabel);
                        var indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel++;
                        foreach (var field in type.GetFields (BindingFlags.Instance | BindingFlags.Public)) {
                            DrawTypeField (component, field, _observer);
                        }
                        EditorGUI.indentLevel = indent;
                    }
                    GUILayout.EndVertical ();
                    EditorGUILayout.Space ();
                }
            }
        }

        void DrawTypeField (object instance, FieldInfo field, EcsEntityObserver entity) {
            var fieldValue = field.GetValue (instance);
            var fieldType = field.FieldType;
            if (!EcsComponentInspectors.Render (field.Name, fieldType, fieldValue, entity)) {
                if (fieldType == typeof (UnityEngine.Object) || fieldType.IsSubclassOf (typeof (UnityEngine.Object))) {
                    GUILayout.BeginHorizontal ();
                    EditorGUILayout.LabelField (field.Name, GUILayout.MaxWidth (EditorGUIUtility.labelWidth - 16));
                    var guiEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField (fieldValue as UnityEngine.Object, fieldType, false);
                    GUI.enabled = guiEnabled;
                    GUILayout.EndHorizontal ();
                    return;
                }
                var strVal = fieldValue != null ? string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", fieldValue) : "null";
                if (strVal.Length > MaxFieldToStringLength) {
                    strVal = strVal.Substring (0, MaxFieldToStringLength);
                }
                GUILayout.BeginHorizontal ();
                EditorGUILayout.LabelField (field.Name, GUILayout.MaxWidth (EditorGUIUtility.labelWidth - 16));
                EditorGUILayout.SelectableLabel (strVal, GUILayout.MaxHeight (EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal ();
            }
        }
    }

    static class EcsComponentInspectors {
        static readonly Dictionary<Type, IEcsComponentInspector> Inspectors = new Dictionary<Type, IEcsComponentInspector> ();

        static EcsComponentInspectors () {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies ()) {
                foreach (var type in assembly.GetTypes ()) {
                    if (typeof (IEcsComponentInspector).IsAssignableFrom (type) && !type.IsInterface) {
                        if (Activator.CreateInstance (type) is IEcsComponentInspector inspector) {
                            var componentType = inspector.GetFieldType ();
                            if (Inspectors.ContainsKey (componentType)) {
                                Debug.LogWarningFormat ("Inspector for \"{0}\" already exists, new inspector will be used instead.", componentType.Name);
                            }
                            Inspectors[componentType] = inspector;
                        }
                    }
                }
            }
        }

        public static bool Render (string label, Type type, object value, EcsEntityObserver observer) {
            if (Inspectors.TryGetValue (type, out var inspector)) {
                inspector.OnGUI (label, value, observer.World, ref observer.Entity);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Custom inspector for specified field type.
    /// </summary>
    public interface IEcsComponentInspector {
        /// <summary>
        /// Supported field type.
        /// </summary>
        Type GetFieldType ();

        /// <summary>
        /// Renders provided instance of specified type.
        /// </summary>
        /// <param name="label">Label of field.</param>
        /// <param name="value">Value of field.</param>
        /// <param name="world">World instance.</param>
        /// <param name="entityId">Entity id.</param>
        // ReSharper disable once InconsistentNaming
        void OnGUI (string label, object value, EcsWorld world, ref EcsEntity entityId);
    }
}