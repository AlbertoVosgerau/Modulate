using UnityEngine;

namespace DandyDino.Modulate
{
    public class ClassA : MonoBehaviour
    {
        private ServiceA _serviceA;

        [Inject]
        public void Init(ServiceB serviceB)
        {
            
        }
    }
}