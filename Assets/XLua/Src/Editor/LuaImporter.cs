#if UNITY_2018_1_OR_NEWER

using UnityEngine;
using UnityEditor;

using System.IO;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

[ScriptedImporter(2, new[] { "lua" })]
public class LuaImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var text = File.ReadAllText(ctx.assetPath);
        var asset = new TextAsset(text);

        ctx.AddObjectToAsset("main obj", asset, GetIconTexture());
        ctx.SetMainObject(asset);
    }

    private Texture2D GetIconTexture()
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/XLua/Editor/lua.png");
    }
}

#endif
