using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

namespace DandyDino.Modulate
{
    [RequireComponent(typeof(ContainerScope)), DefaultExecutionOrder(-1000)]
    public class ModulateViewsContainer : MonoBehaviour, IInstaller
    {
        [Inject] private readonly IEnumerable<IManager> _viewManagers;
        [SerializeField] private List<string> _openViewComponents = new List<string>();
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            IEnumerable<IManager> managers = (IEnumerable<IManager>)containerBuilder.Parent.Resolve(typeof(IEnumerable<IManager>));
            HashSet<Type> managedViewTypes = new HashSet<Type>(managers.Select(m => m.ViewType).Where(t => t != null));

            containerBuilder.RegisterValue(this);
            
            IView[] views = GetComponents<IView>();

            foreach (IView view in views)
            {
                Type viewType = view.GetType();
            
                bool hasManager = managedViewTypes.Any(managed => managed.IsAssignableFrom(viewType));

                if (hasManager)
                {
                    containerBuilder.RegisterValue(view);
                    Debug.Log($"[Modulate] View {view.GetType().Name} registered");
                    continue;
                }
            
                if (view is MonoBehaviour mb)
                {
                    DestroyImmediate(mb);
                    Debug.LogWarning($"[Modulate] Disabling {viewType.Name} — no active IManager handles it.");
                }
            }
        }
        
        private void Reset()
        {
            DestroyIfExists();
        }
        
        private void OnValidate()
        {
            enabled = true;
        }

        private void DestroyIfExists()
        {
            ModulateViewsContainer existing = FindAnyObjectByType<ModulateViewsContainer>();
        
            if (existing != null && existing != this)
            {
                Debug.LogWarning($"Can't have more than one Views Container");
                gameObject.DestroySelf();
            }
        }
    }
}
