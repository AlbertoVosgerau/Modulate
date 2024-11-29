using System;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Provider : MonoBehaviour, IDependencyProvider
    {
        private FactoryA _factoryA;

        [Provide]
        public ServiceA ProvideServiceA()
        {
            return new ServiceA();
        }

        [Provide]
        public ServiceB ProvideServiceB()
        {
            return new ServiceB();
        }

        [Provide]
        public FactoryA ProvideFactoryA()
        {
            if (_factoryA == null)
            {
                _factoryA = new FactoryA();
            }
            return _factoryA;
        }
    }

    public class ServiceA
    {
        [Inject] public TestEnviromentSystem _env;
        public void TestService()
        {
            Debug.Log($"Env was injected in ServiceA: {_env != null}");
        }
    }

    public class ServiceB
    {
        public void Initialize(string message = null)
        {
            Debug.Log($"Initialized Service B with message {message}");
        }
    }

    public class FactoryA
    {
        private ServiceA _cachedService;

        public ServiceA CreateService()
        {
            if (_cachedService == null)
            {
                _cachedService = new ServiceA();
            }

            return _cachedService;
        }
    }
}