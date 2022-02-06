using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using CSObjectWrapEditor;
using Newtonsoft.Json;
using Saro;
using Saro.IO;
using Saro.Lua;
using Saro.Utility;
using Saro.XAsset;
using Saro.XAsset.Build;
using Saro.XAsset.Update;
using UnityEditor;
using UnityEngine;


namespace Tetris
{
    public sealed class BuildMethods : IBuildProcessor
    {
        [XAssetBuildMethod(19, "XLua Gen")]
        public static void XLuaGenCode()
        {
            Generator.ClearAll();
            Generator.GenAll();
        }

        [XAssetBuildMethod(30, "打包lua")]
        public static void PackLuaScripts()
        {
            try
            {
                var luaFiles = new List<string>();
                var dirs = Directory.GetDirectories(Application.dataPath, "LuaScripts", SearchOption.AllDirectories);
                foreach (var dir in dirs)
                {
                    Log.INFO(dir);

                    var luaPaths = Directory.GetFiles(dir, "*.lua", SearchOption.AllDirectories);
                    foreach (var luaPath in luaPaths)
                    {
                        if (luaPath.EndsWith(".meta")) continue;
                        luaFiles.Add(luaPath);
                    }
                }

                if (luaFiles.Count > 0)
                {
                    var script_vfs_path = XAssetPath.k_Editor_DlcOutputPath + "/" + XAssetPath.k_CustomFolder + "/" + VFSLuaLoader.s_ScriptsFileName;

                    var directory = Path.GetDirectoryName(script_vfs_path);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (File.Exists(script_vfs_path))
                    {
                        File.Delete(script_vfs_path);
                    }

                    using (var vfs = VFileSystem.Open(script_vfs_path, FileMode.CreateNew, FileAccess.ReadWrite, luaFiles.Count, luaFiles.Count))
                    {
                        var buffer = new byte[1024 * 100];
                        //byte[] key = new byte[10] { 110, 2, 3, 4, 255, 6, 44, 8, 94, 10 };
                        for (int i = 0; i < luaFiles.Count; i++)
                        {
                            // 基于unity project的相对路径，来处理吧
                            string luaPath = "Assets" + luaFiles[i].Replace(Application.dataPath, "").Replace("\\", "/");

                            //Log.INFO($"{luaFiles[i]} => {luaPath}");

                            var luaFileName = LuaSettings.GetFilename(luaPath);

                            using (var fs = new FileStream(luaPath, FileMode.Open, FileAccess.Read))
                            {
                                //var len = fs.Read(buffer, 0, (int)fs.Length);
                                //EncryptionUtility.QuickSelfXorBytes(buffer, key);
                                //vfs.WriteFile(luaFileName, buffer, 0, len);

                                vfs.WriteFile(luaFileName, fs);
                            }

                            EditorUtility.DisplayProgressBar("打包lua", $"{luaFileName}  {i + 1}/{luaFiles.Count}", (i + 1) / luaFiles.Count);
                        }

#if UNITY_EDITOR && false
                        var fileInfos = vfs.GetAllFileInfos();
                        var settings = new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented
                        };
                        var json = JsonConvert.SerializeObject(fileInfos, settings);
                        File.WriteAllText(script_vfs_path + ".dump.json", json);
#endif
                    }
                }
                else
                {
                    Log.INFO("没有lua文件需要打包");
                }
            }
            catch (Exception e)
            {
                Log.ERROR(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}

