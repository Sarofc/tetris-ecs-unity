using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HybridCLR.Editor
{
    public static class AotConfig
    {
        public static List<Type> s_AotTypes = new List<Type>
        {
            typeof(UnityEngine.Application),
            typeof(UnityEngine.Debug),
            typeof(UnityEngine.SceneManagement.SceneManager),
            typeof(UnityEngine.Animator),
            typeof(UnityEngine.ParticleSystem),
            typeof(UnityEngine.AnimationCurve),
            typeof(UnityEngine.Graphics),
            typeof(UnityEngine.LineRenderer),

            typeof(UnityEngine.Color),
            typeof(UnityEngine.Rect),
            typeof(UnityEngine.RectInt),
            typeof(UnityEngine.Vector2),
            typeof(UnityEngine.Vector2Int),
            typeof(UnityEngine.Vector3),
            typeof(UnityEngine.Vector3Int),
            typeof(UnityEngine.Matrix4x4),
        };

        public static Func<bool> IsAutoGenLinkXML { get; set; } = () => false;

        internal static void AutoGenLinkXML()
        {
            if (IsAutoGenLinkXML?.Invoke() == false)
            {
                UnityEngine.Debug.LogError("Auto GenLinkXML is false");
                return;
            }

            GenLinkXML();
        }

        [UnityEditor.MenuItem("HybridCLR/Generate LinkXML")]
        public static void GenLinkXML()
        {
            var path = "Assets/HybridCLR/Gen/";
            var fileName = "link.xml";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fullPath = path + fileName;

            var hotFixDLLName = Path.GetFileNameWithoutExtension(HybridCLRUtil.s_HotFixDLL);

            AotGenerator.ScanDLL(Assembly.Load(hotFixDLLName), "UnityEngine", out var scanTypes);

            AotGenerator.GenerateLinkXML(scanTypes.Union(s_AotTypes), fullPath);
        }
    }
}
