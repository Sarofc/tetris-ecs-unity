using IFix.Core;
using Newtonsoft.Json;
using Saro;
using System;
using System.Collections.Generic;
using System.IO;

namespace IFix
{
    public class IFixManager
    {
        public static void Patch(string jsonFile)
        {
            var directory = Path.GetDirectoryName(jsonFile);

            try
            {
                var json = File.ReadAllText(jsonFile);

                var pacthes = JsonConvert.DeserializeObject<List<PatchFile>>(json);

                foreach (var patch in pacthes)
                {
                    if (patch.enable)
                    {
                        using (var fs = new FileStream(directory + "/" + patch.name, FileMode.Open, FileAccess.Read))
                        {
                            var vm = PatchManager.Load(fs, patch.checkNew);
                            if (vm != null)
                            {
                                Log.INFO($"[IFix] load {patch.name} success");
                            }
                            else
                            {
                                Log.ERROR($"[IFix] load {patch.name} failed");
                            }
                        }
                    }
                    else
                    {
                        Log.INFO($"[IFix] skip {patch.name}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.ERROR(e);
            }
        }
    }

    [Serializable]
    public class PatchFile
    {
        /// <summary>
        /// 补丁名字
        /// </summary>
        public string name;
        /// <summary>
        /// 补丁开启
        /// </summary>
        public bool enable;
        /// <summary>
        /// 补丁检测 新字段/方法/类
        /// </summary>
        public bool checkNew;
    }
}
