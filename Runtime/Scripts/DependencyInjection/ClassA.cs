using System;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ClassA : MonoBehaviour, IDependencyProvider
    {
        private ServiceB _serviceB;
        
        [Provide]
        ClassA ClassAProvider()
        {
            return this;
        }

        [Inject]
        public void Init(ServiceB serviceB)
        {
            _serviceB = serviceB;
        }

        private void Update()
        {
            Debug.Log($"Service B is injected: {_serviceB != null}");
        }
    }
}