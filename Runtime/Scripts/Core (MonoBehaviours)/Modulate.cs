using System;
using System.Collections.Generic;
using System.Linq;
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

            _main.InitializeServices();
        }

        public static Modulate Main => _main;
        private static Modulate _main;

        public ManagerContainer ManagerContainer => _managerContainer;
        private ManagerContainer _managerContainer;

        public ServicesCollection Services => _services;
        [SerializeReference] private ServicesCollection _services;

        private void InitializeServices()
        {
            _services = Resources.Load<ServicesCollection>("ServicesCollection");
            foreach (GameService service in _services.gameServices)
            {
                service.InitAsync();
            }
        }

        public List<GameService> GetAllServices()
        {
            return Services.gameServices;
        }

        public List<GameService> GetAllActiveServices()
        {
            return Services.gameServices.Where(x => x.IsEnabled).ToList();
        }

        public T GetService <T>(bool forceEnable = false) where T : GameService
        {
            T service = Services.gameServices.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
            if (service == null)
            {
                return null;
            }
            
            if (!service.IsEnabled)
            {
                if (forceEnable)
                {
                    service.SetEnabled(true);
                    return service;
                }
                Debug.LogWarning($"Service of type {typeof(T).Name} exists but it is not enabled");
                return null;
            }

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
                manager.onAskForDisposal += _managerContainer.OnManagerNeedsDispose;
                managerContainer.gameObject.DestroySelf();
            }
        }
        

        private void OnDisable()
        {
            foreach (GameService service in Services.gameServices)
            {
                if (!service.IsEnabled)
                {
                    continue;
                }
                
                service.OnDisable();
            }
        }

        private void OnDestroy()
        {
            foreach (GameService service in Services.gameServices)
            {
                service.OnDestroy();
            }
        }

        private void Update()
        {
            foreach (GameService service in Services.gameServices)
            {
                if (!service.IsEnabled)
                {
                    continue;
                }
                service.Update();
            }
        }
    }
}