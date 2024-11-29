using UnityEngine;

namespace DandyDino.Modulate
{
    public class ServiceA
    {
        private TestEnviromentSystem _env;

        public ServiceA(TestEnviromentSystem env)
        {
            _env = env;
        }
        public void TestService()
        {
            Debug.Log($"Env was injected in ServiceA: {_env != null}");
        }
    }
}