using UnityEngine;

namespace DandyDino.Modulate
{
    public class TestEnviromentSystem : MonoBehaviour, IDependencyProvider
    {
        [Provide]
        TestEnviromentSystem ProvideEnviromentSystem()
        {
            return this;
        }
    }
}
