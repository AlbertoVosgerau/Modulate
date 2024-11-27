using System;
using System.Collections.Generic;
using UnityEngine;

namespace DandyDino.Modulate
{
    [AddComponentMenu("")]
    public class Modulate : MonoBehaviour
    {
        private static bool _isInitialized;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 
        public static void Init()
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

        public List<IService> GetAllServices()
        {
            return ServiceFactory.GetAllServices();
        }

        public T GetService <T>() where T : class, IService, new()
        {
            T service = ServiceFactory.GetService<T>();
            return service;
        }
        
        public T GetManager <T>(bool forceEnable = false) where T : class, IManager
        {
            try
            {
                T manager = _managerContainer.GetManager<T>();
                if (manager != null && forceEnable)
                {
                    manager.SetEnabled(true);
                }
                return manager;
            }
            catch (Exception e)
            {
                throw new Exception($"Can't retrieve Manager {typeof(T).Name}. Please make sure you have set the correct dependency.\n{e.Message}");
            }
        }

        public void RegisterManagerContainer(ManagerContainer managerContainer)
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