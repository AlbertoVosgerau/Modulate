using System;
using System.Collections.Generic;
using UnityEngine;

namespace DandyDino.Modulate
{
    [DisallowMultipleComponent, AddComponentMenu(""),  DefaultExecutionOrder(-400)]
    public sealed class Modulate : MonoBehaviour
    {
        private static bool _isInitialized;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 
        internal static void Init()
        {
            if (_isInitialized || !Application.isPlaying)
            {
                return;
            }
            _isInitialized = true;
            
            GameObject newObject = new GameObject(StringLibrary.MODULATE_NAME);
            _main = newObject.AddComponent<Modulate>();
            DontDestroyOnLoad(newObject);
        }

        public static Modulate Main => _main;
        private static Modulate _main;

        public ManagerContainer ManagerContainer => _managerContainer;
        private ManagerContainer _managerContainer;

        /// <summary>
        /// Gets all existing Game Services
        /// </summary>
        /// <returns>List of all initialized Game Services in game</returns>
        public List<IService> GetAllGameServices()
        {
            return GameServicesFactory.GetAllGameServices();
        }

        /// <summary>
        /// Get a specific Game Service
        /// </summary>
        /// <typeparam name="T">Concrete implementation of IService, effectively any Game Service created in your game that you have access to</typeparam>
        /// <returns>If exists, returns the Game Service of the desire type. Returns null instead.</returns>
        public T GetGameService <T>() where T : class, IService, new()
        {
            T service = GameServicesFactory.GetGameService<T>();
            return service;
        }

        /// <summary>
        /// When disposing a service, remove from the factory dictionary
        /// </summary>
        /// <param name="service">Concrete implementation of a Game Service to dispose</param>
        public void DisposeGameService(IService service)
        {
            GameServicesFactory.RemoveService(service);
        }
        
        /// <summary>
        /// Get a Manager from current active Manager Container
        /// </summary>
        /// <param name="forceEnable">Should the manager to be forced enabled if found?</param>
        /// <typeparam name="T">Concrete implementation the Manager</typeparam>
        /// <returns>If exists, returns the Manager of the desire type. Returns null instead</returns>
        public T GetManager <T>(bool forceEnable = false) where T : class, IManager
        {
            T manager = _managerContainer.GetManager<T>();
            if (manager != null && forceEnable)
            {
                manager.SetEnabled(true);
            }
            return manager;
        }
        
        
        internal void RegisterManagerContainer(ManagerContainer managerContainer)
        {
            if (_managerContainer == null && _managerContainer != managerContainer)
            {
                _managerContainer = managerContainer;
                DontDestroyOnLoad(_managerContainer);
                return;
            }

            foreach (IManager manager in managerContainer.Managers)
            {
                bool hasManagerOfType = false;
                
                foreach (IManager existingManager in _managerContainer.Managers)
                {
                    if (existingManager.GetType() == manager.GetType())
                    {
                        Debug.LogWarning($"Manager Container already contains {manager.GetType().Name}. Will destroy the newly loaded. Please verify if that was your intention.");
                        hasManagerOfType = true;
                        break;
                    }
                }
                
                if (hasManagerOfType)
                {
                    continue;
                }
                
                _managerContainer.Managers.Add(manager);
                managerContainer.gameObject.DestroySelf();
            }
        }
    }
}