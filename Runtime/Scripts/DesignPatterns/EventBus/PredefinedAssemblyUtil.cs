using System;
using System.Collections.Generic;
using System.Reflection;

namespace DandyDino.Modulate
{
    /// <summary>
    /// A utility class, PredefinedAssemblyUtil, provides methods to interact with predefined assemblies.
    /// It allows to get all types in the current AppDomain that implement from a specific Interface type.
    /// For more details, <see href="https://docs.unity3d.com/2023.3/Documentation/Manual/ScriptCompileOrderFolders.html">visit Unity Documentation</see>
    /// </summary>
    public static class PredefinedAssemblyUtil
    {
        /// <summary>
        /// Gets all Types from all assemblies in the current AppDomain that implement the provided interface type.
        /// </summary>
        /// <param name="interfaceType">Interface type to get all the Types for.</param>
        /// <returns>List of Types implementing the provided interface type.</returns>    
        public static List<Type> GetTypes(Type interfaceType)
        {
            var results = new List<Type>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Skip Unity/system assemblies for speed
                var name = asm.GetName().Name;
                if (name.StartsWith("Unity") || name.StartsWith("System") ||
                    name.StartsWith("Mono.") || name == "mscorlib" || name == "netstandard")
                    continue;

                Type[] types;
                try { types = asm.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types; }

                foreach (var t in types)
                {
                    if (t != null && t != interfaceType && interfaceType.IsAssignableFrom(t))
                        results.Add(t);
                }
            }
            return results;
        }
    }
}