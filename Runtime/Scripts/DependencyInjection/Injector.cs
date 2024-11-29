using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DandyDino.Modulate
{
    [DisallowMultipleComponent, AddComponentMenu(""),  DefaultExecutionOrder(-500)]
    public class Injector : MonoBehaviour
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static Injector System => _system;
        private static Injector _system;
        private Dictionary<Type, object> _registry = new Dictionary<Type, object>();
        private List<IDependencyProvider> _providers;

        /// <summary>
        /// Add a Dependency Provider to the system
        /// </summary>
        public void AddProvider(IDependencyProvider provider)
        {
            if (_providers.Contains(provider))
            {
                return;
            }

            Debug.Log($"Add provider {provider}");
            _providers.Add(provider);
            UpdateProviders();
        }

        /// <summary>
        /// Remove a Dependency Provider from the system
        /// </summary>
        public void RemoveProvider(IDependencyProvider provider)
        {
            if (!_providers.Contains(provider))
            {
                return;
            }

            _providers.Remove(provider);
            UpdateProviders();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] 
        private static void Init()
        {
            Debug.Log($"Starting Injector");
            if (_system != null && Application.isPlaying)
            {
                Destroy(_system.gameObject);
            }
            
            GameObject newObject = new GameObject(StringLibrary.INJECTOR_NAME);
            newObject.AddComponent<Injector>();
        }

        private void Awake()
        {
            _system = GetComponent<Injector>();
            _providers = _system.GetValidObjects().OfType<IDependencyProvider>().ToList();
            UpdateProviders();
            DontDestroyOnLoad(this);
        }

        private void UpdateProviders()
        {
            foreach (IDependencyProvider provider in _providers)
            {
                RegisterProvider(provider);
            }

            IEnumerable<object> injectables = GetValidObjects().Where(x => IsInjectable(x));

            foreach (var injectable in injectables)
            {
                Inject(injectable);
            }
        }

        private void Inject(object injectable)
        {
            Type type = injectable.GetType();
            IEnumerable<FieldInfo> injectableFields = type.GetFields(BINDING_FLAGS).Where(x => Attribute.IsDefined(x, typeof(InjectAttribute)));

            foreach (FieldInfo fieldInfo in injectableFields)
            {
                var fieldType = fieldInfo.FieldType;
                var resolvedInstance = GetInstance(fieldType);
                if (resolvedInstance == null)
                {
                    return;
                }
                
                fieldInfo.SetValue(injectable, resolvedInstance);
            }
            
            IEnumerable<MethodInfo> injectableMethods = type.GetMethods(BINDING_FLAGS).Where(x => Attribute.IsDefined(x, typeof(InjectAttribute)));

            foreach (MethodInfo methodInfo in injectableMethods)
            {
                IEnumerable<Type> requiredParameters = methodInfo.GetParameters().Select(x => x.ParameterType);

                var resolvedInstances = requiredParameters.Select(GetInstance).ToArray();

                if (resolvedInstances.Any(x => x == null))
                {
                    continue;
                }

                methodInfo.Invoke(injectable, resolvedInstances);
            }
        }

        private object GetInstance(Type type)
        {
            _registry.TryGetValue(type, out object resolvedInstance);
            return resolvedInstance;
        }

        private static bool IsInjectable(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            
            Type type = obj.GetType();
            
            if (typeof(GameObject).IsAssignableFrom(type) ||  (typeof(Component).IsAssignableFrom(type) && !typeof(MonoBehaviour).IsAssignableFrom(type)))
            {
                return false;
            }
            
            MemberInfo[] members = obj.GetType().GetMembers(BINDING_FLAGS);
            return members.Any(x => Attribute.IsDefined(x, typeof(InjectAttribute)));
        }

        private void RegisterProvider(IDependencyProvider provider)
        {
            MethodInfo[] methods = provider.GetType().GetMethods(BINDING_FLAGS);

            foreach (MethodInfo methodInfo in methods)
            {
                if (!Attribute.IsDefined(methodInfo, typeof(ProvideAttribute)))
                {
                    continue;
                }

                var returnType = methodInfo.ReturnType;
                var providedInstance = methodInfo.Invoke(provider, null);
                if (providedInstance != null)
                {
                    _registry.TryAdd(returnType, providedInstance);
                    continue;
                }

                throw new Exception($"Provider {provider.GetType().Namespace} returned null for {returnType.Name}");
            }
        }
        
        private object[] GetValidObjects()
        {
            List<object> objects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).Cast<object>().ToList();
            ManagerContainer managerContainer = Modulate.Main.ManagerContainer;
            if (managerContainer != null)
            {
                objects.AddRange(managerContainer.Managers);
            }
            
            objects.AddRange(Modulate.Main.GetAllGameServices());
            
            return objects.ToArray();
        }
    }
}
