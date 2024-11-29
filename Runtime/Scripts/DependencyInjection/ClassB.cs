using System;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ClassB : MonoBehaviour
    {
        [Inject] private ServiceA _serviceA;

        private FactoryA _factoryA;

        [Inject] private TestEnviromentSystem _env;

        [Inject]
        private void Init(FactoryA fatoryA)
        {
            _factoryA = fatoryA;
        }

        private void Awake()
        {
            
        }

        private void Update()
        {
            Debug.Log($"Env is injected in ClassB: {_env != null}");
            if (_serviceA == null)
            {
                Debug.Log($"Service A is null");
                return;
            }
            
            _serviceA.TestService();
        }
    }
}