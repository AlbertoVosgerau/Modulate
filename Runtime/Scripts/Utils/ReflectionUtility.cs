using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

namespace DandyDino.Modulate
{
    public static class ReflectionUtility
    {
        public static List<Type> GetAllClassesOfType<T>()
        {
            List<Type> result = new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type queryType = typeof(T);

            foreach (Assembly assembly in assemblies)
            {
                Type[] typesInAssembly = assembly.GetTypes();

                foreach (Type type in typesInAssembly)
                {
                    if (type.IsAbstract || type.BaseType == null || type.FullName == null)
                    {
                        continue;
                    }

                    if (queryType.IsInterface)
                    {
                        if (queryType.IsAssignableFrom(type) && !type.IsAbstract)
                        {
                            result.Add(type);
                        }
                    }
                    else
                    {
                        Type baseType = type.BaseType;
                        if (baseType.IsGenericType)
                        {
                            Type[] tArgs = queryType.GetGenericArguments();
                            Type[] args = baseType.GetGenericArguments();
                            bool argsEqual = tArgs.Length > 0 && args.Length > 0 && tArgs[0] == args[0].BaseType;

                            // This is HACKY - Check if the base type matches
                            bool isSubclass = baseType.FullName.Split('`')[0] == queryType.FullName.Split('`')[0];

                            if (isSubclass && argsEqual && !type.IsAbstract)
                            {
                                result.Add(type);
                            }
                        }
                        else
                        {
                            if (queryType.IsAssignableFrom(type) && !type.IsAbstract)
                            {
                                result.Add(type);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static List<T> GetInstancesOfAllClassesOfType<T>() where T : Object, new()
        {
            List<T> result = new List<T>();
            List<Type> types = GetAllClassesOfType<T>();

            foreach (Type type in types)
            {
                object instance= (T)Activator.CreateInstance(type);
                result.Add(instance as T);
            }

            return result;
        }
        
        public static IEnumerable<Type> GetAllManagerTypes()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }

                foreach (Type type in types)
                {
                    if (type == null) continue;
                    if (type.IsAbstract || type.IsInterface) continue;
                    if (type.IsGenericTypeDefinition) continue;

                    if (ImplementsIManager(type))
                    {
                        yield return type;
                    }
                }
            }
        }
        
        private static bool ImplementsIManager(Type type)
        {
            return typeof(IManager).IsAssignableFrom(type);
        }
    }
}