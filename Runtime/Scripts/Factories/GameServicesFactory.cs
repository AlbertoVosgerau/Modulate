using System;
using System.Collections.Generic;

namespace DandyDino.Modulate
{
    internal static class GameServicesFactory
    {
        private static Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
        
        internal static List<IService> GetAllGameServices()
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
        
        internal static T GetGameService<T>() where T : class, IService, new()
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

        internal static void RemoveService(IService service)
        {
            if (_services.ContainsValue(service))
            {
                _services.Remove(service.GetType());
            }
            GC.Collect();
        }
    }
}
