using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using Resolution = Reflex.Enums.Resolution;

namespace DandyDino.Modulate
{
    public class ModulateGameInstanceInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private FeatureToggleAsset _featureToggleAsset;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            _featureToggleAsset = Resources.Load<FeatureToggleAsset>("Feature ToggleAsset");
            Refresh();
            RegisterAllManagers(containerBuilder);
        }

        private void RegisterAllManagers(ContainerBuilder containerBuilder)
        {
            foreach (IManager manager in _featureToggleAsset.Managers)
            {
                if (manager == null) continue;

                Type concreteType = manager.GetType();
                if (!_featureToggleAsset.IsEnabled(concreteType))
                {
                    continue;
                }

                containerBuilder.RegisterType(
                    concreteType,
                    new[] { concreteType, typeof(IManager) },
                    Lifetime.Singleton,
                    Resolution.Eager
                );
            }
        }

        public List<IManager> Refresh()
        {
            _featureToggleAsset.Refresh();
            return _featureToggleAsset.Managers;
        }
    }
}