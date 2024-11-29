using System;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class TestEnviromentSystem : ModulateDependencyProvider
    {
        [Provide]
        TestEnviromentSystem ProvideEnviromentSystem()
        {
            return this;
        }
    }
}
