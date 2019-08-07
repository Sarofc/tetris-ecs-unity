using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ReflectionEx {
    private static IEnumerable<Type> m_AllAssemblyTypes;

    public static IEnumerable<Type> GetAllAssemblyTypes {
        get {
            if (m_AllAssemblyTypes == null) {
                m_AllAssemblyTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (
                    t => {
                        try {
                            return t.GetTypes ();
                        } catch {
                            return new Type[0];
                        }
                    }
                );
            }
            return m_AllAssemblyTypes;
        }
    }
}