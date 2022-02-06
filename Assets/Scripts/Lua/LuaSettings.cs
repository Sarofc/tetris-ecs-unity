#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

using Object = UnityEngine.Object;

namespace Saro.Lua
{
    [Serializable]
    public class LuaSettings : ScriptableObject
    {
        public const string k_LUA_SETTINGS_PATH = "Assets/Editor/XLua/LuaSettings.asset";

        [SerializeField]
        private List<DefaultAsset> m_SrcRoots = new List<DefaultAsset>();

        public List<string> SrcRoots
        {
            get
            {
                if (this.m_SrcRoots == null)
                    return new List<string>();
                return this.m_SrcRoots.Where(asset => asset != null).Select(asset => AssetDatabase.GetAssetPath(asset)).ToList();
            }
        }

        public static string GetFilename(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            int start = path.LastIndexOf("/");
            int dotIndex = path.IndexOf(".", start);
            if (dotIndex > -1)
                path = path.Substring(0, dotIndex);

            LuaSettings luaSettings = LuaSettings.GetOrCreateSettings();
            foreach (string root in luaSettings.SrcRoots)
            {
                if (path.StartsWith(root))
                {
                    path = path.Replace(root + "/", "").Replace("/", ".");
                    return path;
                }
            }

            int index = path.IndexOf("Resources");
            if (index >= 0)
                path = path.Substring(index + 10);

            path = path.Replace("/", ".");
            return path;
        }

        public static string GetFilename(Object asset)
        {
            if (asset == null)
                return null;

            string path = AssetDatabase.GetAssetPath(asset);
            return GetFilename(path);
        }

        public static LuaSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<LuaSettings>(k_LUA_SETTINGS_PATH);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LuaSettings>();
                FileInfo file = new FileInfo(k_LUA_SETTINGS_PATH);
                if (!file.Directory.Exists)
                    file.Directory.Create();

                AssetDatabase.CreateAsset(settings, k_LUA_SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}
#endif