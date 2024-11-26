using System.Collections.Generic;
using System.Linq;
using DandyDino.Modulate;
using UnityEngine;

public class ManagerContainer : MonoBehaviour
{
    [SerializeReference] public List<IManager> Managers = new List<IManager>();

    public void Init()
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

    public T GetManager <T>(bool forceEnabled = false) where T : class, IManager
    {
        T manager = Managers.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
        
        if (manager != null)
        {
            if (!manager.IsEnabled)
            {
                if (forceEnabled)
                {
                    manager.SetEnabled(true);
                    return manager;
                }
                Debug.LogWarning($"Manager of type {typeof(T).Name} exists but it is not enabled");
                return null;
            }
            return manager;
        }

        return null;
    }

    public void AddManager(IManager manager)
    {
        if (!Managers.Contains(manager))
        {
            manager.InitAsync();
            Managers.Add(manager);
        }
    }

    public void RemoveManager(IManager manager)
    {
        if (Managers.Contains(manager))
        {
            manager.Destroy();
            Managers.Remove(manager);
        }
    }

    public void OnManagerNeedsDispose(IManager manager)
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
