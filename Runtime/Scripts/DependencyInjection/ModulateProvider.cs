using System;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ModulateProvider : MonoBehaviour, IDependencyProvider
    {
        protected virtual void OnEnable()
        {
            Injector.System.AddProvider(this);
        }

        protected virtual void OnDisable()
        {
            Injector.System.AddProvider(this);
        }
    }
}
