using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DandyDino.Modulate;
using UnityEngine;
[assembly: InternalsVisibleTo(StringLibrary.ASSEMBLY_DEFINITION_EDITOR)]

namespace DandyDino.Modulate
{
    [DisallowMultipleComponent]
    
    public class ManagerContainer : MonoBehaviour
    {
        [SerializeReference] internal List<IManager> Managers = new List<IManager>();
    
        internal void Init()
        {
            for (int i = 0; i < Managers.Count; i++)
            {
                IManager manager = Managers[i];
            
                if (!manager.IsEnabled)
                {
                    continue;
                }

                manager.InitAsync();
                manager.RegisterScenes(gameObject.scene);
                manager.onAskForDisposal += OnManagerNeedsDispose;
            }

            Modulate.Main.RegisterManagerContainer(this);
        }

        /// <summary>
        /// Get a manager registered in this Manager Container
        /// </summary>
        /// <param name="forceEnabled">If the Manager is disabled, should force it to be enabled?</param>
        /// <typeparam name="T">Concrete type implementation of the Manager</typeparam>
        /// <returns>If it exists, returns the Manager of the requested type</returns>
        public T GetManager <T>(bool forceEnabled = false) where T : class, IManager
        {
            T manager = Managers.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
        
            if (manager != null)
            {
                if (!manager.IsEnabled && forceEnabled)
                {
                    manager.SetEnabled(true);
                }
                return manager;
            }

            return null;
        }

        /// <summary>
        /// Adds a Manager to this container. Skips it if it already exists.
        /// </summary>
        /// <param name="manager">Concrete implementation of the interface IManager</param>
        public void AddManager(IManager manager)
        {
            if (!Managers.Contains(manager))
            {
                manager.InitAsync();
                Managers.Add(manager);
            }
        }

        /// <summary>
        /// Removes a Manager from this Manager Container if it exits
        /// </summary>
        /// <param name="manager"></param>
        public void RemoveManager(IManager manager)
        {
            if (Managers.Contains(manager))
            {
                manager.Destroy();
                Managers.Remove(manager);
            }
        }


        private void OnManagerNeedsDispose(IManager manager)
        {
            if (!Managers.Contains(manager))
            {
                return;
            }

            Managers.Remove(manager);
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            for (int i = 0; i < Managers.Count; i++)
            {
                if (!Managers[i].IsEnabled)
                {
                    continue;
                }
                Managers[i].SetEnabled(false);
            }
        }

        private void Update()
        {
            for (int i = 0; i < Managers.Count; i++)
            {
                if (!Managers[i].IsEnabled)
                {
                    continue;
                }
                Managers[i].Update();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < Managers.Count; i++)
            {
                if (!Managers[i].IsEnabled)
                {
                    continue;
                }
                Managers[i].LateUpdate();
            }
        }
    
        private void FixedUpdate()
        {
            for (int i = 0; i < Managers.Count; i++)
            {
                if (!Managers[i].IsEnabled)
                {
                    continue;
                }
                Managers[i].FixedUpdate();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < Managers.Count; i++)
            {
                Managers[i].Destroy();
            }
        }
    }
}