using System.Reflection;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace DandyDino.Modulate
{
    public class ModulateViewsContainer : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue(this);
            
            IView[] views = GetComponents<IView>();

            foreach (IView view in views)
            {
                containerBuilder.RegisterValue(view);
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

        private void Awake()
        {
            DestroyIfExists();
        }
        
        private void DestroyIfExists()
        {
            ModulateViewsContainer existing = FindAnyObjectByType<ModulateViewsContainer>();

            if (existing != null && existing != this)
            {
                CopyViewsTo(existing);
                
                Debug.LogWarning($"Can't have more than one Views Container");
                Destroy(gameObject);
            }
        }
        
        private void CopyViewsTo(ModulateViewsContainer target)
        {
            IView[] views = GetComponents<IView>();
            
            foreach (IView view in views)
            {
                MonoBehaviour sourceComponent = view as MonoBehaviour;
                if (sourceComponent == null) continue;
                
                System.Type type = sourceComponent.GetType();
                Component newComponent = target.gameObject.AddComponent(type);
                
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(newComponent, field.GetValue(sourceComponent));
                }
            }
        }
    }
}
