using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Saro.Lua
{
    public enum EScriptReferenceType
    {
        TextAsset,
        Filename
    }

    [System.Serializable]
    public class LuaScriptReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField]
        private Object cachedAsset;
#endif

        [SerializeField]
        protected TextAsset text;

        [SerializeField]
        protected string filename;

        [SerializeField]
        protected EScriptReferenceType type = EScriptReferenceType.TextAsset;

        public virtual EScriptReferenceType Type
        {
            get { return this.type; }
        }

        public virtual TextAsset Text
        {
            get { return this.text; }
        }

        public virtual string Filename
        {
            get { return this.filename; }
        }

        public void OnAfterDeserialize()
        {
            Clear();
        }

        public void OnBeforeSerialize()
        {
            Clear();
        }

        protected virtual void Clear()
        {
#if !UNITY_EDITOR
            switch (type)
            {
                case EScriptReferenceType.TextAsset:
                    this.filename = null;
                    break;
                case EScriptReferenceType.Filename:
                    this.text = null;
                    break;
            }
#endif
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(LuaScriptReference))]
    public class ScriptReferenceDrawer : PropertyDrawer
    {
        private const float HORIZONTAL_GAP = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var objectProperty = property.FindPropertyRelative("cachedAsset");
            var typeProperty = property.FindPropertyRelative("type");
            var textProperty = property.FindPropertyRelative("text");
            var filenameProperty = property.FindPropertyRelative("filename");

            float y = position.y;
            float x = position.x;
            float height = GetPropertyHeight(property, label);
            float width = position.width - HORIZONTAL_GAP * 2;

            Rect nameRect = new Rect(x, y, 60, height);
            Rect typeRect = new Rect(nameRect.xMax + HORIZONTAL_GAP, y, 80, height);
            Rect valueRect = new Rect(typeRect.xMax + HORIZONTAL_GAP, y, position.xMax - typeRect.xMax - HORIZONTAL_GAP, height);

            EditorGUI.LabelField(nameRect, property.displayName);

            Object asset = objectProperty.objectReferenceValue;
            EScriptReferenceType typeValue = (EScriptReferenceType)typeProperty.enumValueIndex;
            EditorGUI.BeginChangeCheck();
            EScriptReferenceType newTypeValue = (EScriptReferenceType)EditorGUI.EnumPopup(typeRect, typeValue);
            if (EditorGUI.EndChangeCheck() && typeValue != newTypeValue)
            {
                if (ValidateSetting(asset, newTypeValue))
                {
                    typeProperty.enumValueIndex = (int)newTypeValue;
                    UpdateProperty(filenameProperty, textProperty, newTypeValue, asset);
                }
            }

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.1f;

            EditorGUI.BeginChangeCheck();
            Object newAsset = null;
            switch (newTypeValue)
            {
                case EScriptReferenceType.Filename:
                    {
                        if (asset != null)
                        {
                            var name = asset.name;
                            asset.name = filenameProperty.stringValue;
                            newAsset = EditorGUI.ObjectField(valueRect, GUIContent.none, asset, typeof(Object), false);
                            asset.name = name;
                        }
                        else
                        {
                            newAsset = EditorGUI.ObjectField(valueRect, GUIContent.none, asset, typeof(Object), false);
                        }
                        break;
                    }
                case EScriptReferenceType.TextAsset:
                    {
                        if (asset is TextAsset)
                            newAsset = EditorGUI.ObjectField(valueRect, GUIContent.none, asset, typeof(TextAsset), false);
                        else
                            newAsset = EditorGUI.ObjectField(valueRect, GUIContent.none, null, typeof(TextAsset), false);
                        break;
                    }
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (ValidateAsset(newAsset) && ValidateSetting(newAsset, newTypeValue))
                {
                    objectProperty.objectReferenceValue = newAsset;
                    UpdateProperty(filenameProperty, textProperty, newTypeValue, newAsset);
                }
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.EndProperty();
        }

        protected virtual bool ValidateAsset(Object asset)
        {
            if (asset == null)
                return true;

            if (!(asset is TextAsset || asset is DefaultAsset))
            {
                Debug.LogWarningFormat("Invalid asset for ScriptReference");
                return false;
            }

            string path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path))
                return false;

            if (asset is DefaultAsset && Directory.Exists(path))
            {
                Debug.LogWarningFormat("Invalid asset for ScriptReference path = '{0}'.", path);
                return false;
            }

            if (path.EndsWith(".cs"))
            {
                Debug.LogWarningFormat("Invalid asset for ScriptReference path = '{0}'.", path);
                return false;
            }
            return true;
        }

        protected virtual bool ValidateSetting(Object asset, EScriptReferenceType type)
        {
            if (asset == null || type == EScriptReferenceType.TextAsset)
                return true;

            string path = AssetDatabase.GetAssetPath(asset);
            LuaSettings luaSettings = LuaSettings.GetOrCreateSettings();
            foreach (string root in luaSettings.SrcRoots)
            {
                if (path.StartsWith(root))
                    return true;
            }

            if (path.IndexOf("Resources") >= 0)
                return true;

            if (EditorUtility.DisplayDialog("Notice", string.Format("The file \"{0}\" is not in the source code folder of lua. Do you want to add a source code folder?", asset.name), "Yes", "Cancel"))
            {
                SettingsService.OpenProjectSettings("Project/LuaSettingsProvider");
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual void UpdateProperty(SerializedProperty filenameProperty, SerializedProperty textProperty, EScriptReferenceType type, Object asset)
        {
            switch (type)
            {
                case EScriptReferenceType.TextAsset:
                    if (asset != null && asset is TextAsset)
                        textProperty.objectReferenceValue = (TextAsset)asset;
                    else
                        textProperty.objectReferenceValue = null;
                    filenameProperty.stringValue = null;
                    break;
                case EScriptReferenceType.Filename:
                    if (asset != null)
                        filenameProperty.stringValue = LuaSettings.GetFilename(asset);
                    else
                        filenameProperty.stringValue = null;
                    textProperty.objectReferenceValue = null;
                    break;
            }
        }
    }

#endif
}
