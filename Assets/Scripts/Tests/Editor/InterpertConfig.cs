
using IFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IFix
{
    [Configure]
    public class InterpertConfig
    {
        [IFix]
        static IEnumerable<Type> ToProcess
        {
            get
            {
                return from assmbly in AppDomain.CurrentDomain.GetAssemblies()
                       from type in assmbly.GetTypes()
                       select type;

                //return (from type in Assembly.Load("Assembly-CSharp").GetTypes()
                //        where type.Namespace != "XLua" && !type.Name.Contains("<")
                //        select type);
            }
        }
    }

}
