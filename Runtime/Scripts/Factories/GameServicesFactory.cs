using System;
using System.Collections.Generic;

namespace DandyDino.Modulate
{
    internal class GameServicesFactory
    {
        public static GameServicesFactory Factory => _factory ??= new GameServicesFactory();
        private static GameServicesFactory _factory;
        
        private Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
        
        internal List<IService> GetAllGameServices()
        {
            List<IService> allServices = new List<IService>();
            foreach (IService service in _services.Values)
            {
                if (service is IService gameService)
                {
                    allServices.Add(gameService);
                }
            }

            return allServices;
        }
        
        internal T GetGameService<T>() where T : class, IService, new()
        {
            if (_services.ContainsKey(typeof(T)))
            {
                return _services[typeof(T)] as T;
            }

            T newService = new T();
            newService.InitAsync();
            _services.Add(typeof(T), newService);
            return newService;
        }

        internal void RemoveService(IService service)
        {
            if (_services.ContainsValue(service))
            {
                _services.Remove(service.GetType());
            }
            GC.Collect();
        }
    }
}
