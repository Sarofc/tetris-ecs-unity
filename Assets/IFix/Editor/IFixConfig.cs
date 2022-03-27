using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IFix
{
    [Configure]
    public class IFixConfig
    {
        [IFix]
        static IEnumerable<Type> hotfix
        {
            get
            {
                var result = new List<Type>();

                var types1 = from type in Assembly.Load("Assembly-CSharp").GetTypes()
                             where type.Namespace != "XLua" && !type.Name.Contains("<")
                             select type;

                var types2 = from type in Assembly.Load("Saro.MGF").GetTypes()
                             where !type.Name.Contains("<")
                             select type;

                var types3 = from type in Assembly.Load("Saro.XAsset").GetTypes()
                             where !type.Name.Contains("<")
                             select type;

                result.AddRange(types1);
                result.AddRange(types2);
                result.AddRange(types3);
                return result;
            }
        }
    }
}
