using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DandyDino.Modulate
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute
    {
        public InjectAttribute(){}
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute
    {
        public ProvideAttribute(){}
    }


    public interface IDependencyProvider
    {
        // public void RegisterProvider()
        // {
        //     Injector.System.AddProvider(this);
        // }
        //
        // public void UnregisterProvider()
        // {
        //     Injector.System.RemoveProvider(this);
        // }
    }

    [DisallowMultipleComponent, AddComponentMenu(""),  DefaultExecutionOrder(-1000)]
    public class Injector : MonoBehaviour
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static Injector System => _system;
        private static Injector _system;
        private Dictionary<Type, object> _registry = new Dictionary<Type, object>();
        private List<IDependencyProvider> _providers;

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
            _system = newObject.AddComponent<Injector>();
            DontDestroyOnLoad(newObject);
            
            _system.InitializeInternal();
        }

        private void InitializeInternal()
        {
            _providers = _system.GetMonoBehaviours().OfType<IDependencyProvider>().ToList();
            UpdateProviders();
        }

        private void UpdateProviders()
        {
            foreach (IDependencyProvider provider in _providers)
            {
                RegisterProvider(provider);
            }

            IEnumerable<MonoBehaviour> injectables = GetMonoBehaviours().Where(x => IsInjectable(x));

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
                    _registry.Add(returnType, providedInstance);
                    continue;
                }

                throw new Exception($"Provider {provider.GetType().Namespace} returned null for {returnType.Name}");
            }
        }

        // It won't be MonoBehaviours in my system. Use Reflection to get them from Managers or something like that.
        private MonoBehaviour[] GetMonoBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }
    }
}
