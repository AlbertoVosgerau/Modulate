using System.Collections.Generic;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CreateAssetMenu(order = 0, fileName = StringLibrary.SERVICES_COLLECTION_NAME, menuName = StringLibrary.SERVICES_COLLECTION_NAME)]
    public class ServicesCollection : ScriptableObject
    {
        [SerializeReference] public List<IService> gameServices = new List<IService>();
        
        public void AddService(IService service)
        {
            if (gameServices.Contains(service))
            {
                return;
            }
            gameServices.Add(service);
        }

        public void RemoveService(IService service)
        {
            if (!gameServices.Contains(service))
            {
                return;
            }
            gameServices.Remove(service);
        }
    }
}