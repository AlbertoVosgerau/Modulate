using System;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ModulateDependencyProvider : MonoBehaviour, IDependencyProvider
    {
        protected virtual void OnEnable()
        {
            if (Injector.System == null)
            {
                return;
            }
            Injector.System.AddProvider(this);
        }

        protected virtual void OnDisable()
        {
            if (Injector.System == null)
            {
                return;
            }
            
            Injector.System.AddProvider(this);
        }
    }
}
