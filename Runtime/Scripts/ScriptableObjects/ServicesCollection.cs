using System.Collections.Generic;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CreateAssetMenu(order = 0, fileName = StringLibrary.SERVICES_COLLECTION_NAME, menuName = StringLibrary.SERVICES_COLLECTION_NAME)]
    public class ServicesCollection : ScriptableObject
    {
        [SerializeReference] public List<GameService> gameServices = new List<GameService>();
        
        public void AddService(GameService service)
        {
            if (gameServices.Contains(service))
            {
                return;
            }
            gameServices.Add(service);
        }

        public void RemoveService(GameService service)
        {
            if (!gameServices.Contains(service))
            {
                return;
            }
            gameServices.Remove(service);
        }
    }
}