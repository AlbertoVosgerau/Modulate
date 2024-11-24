using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DandyDino.Modulate
{
    [Serializable]
    public abstract class GameService : IService
    {
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onEnable { get; set; }
        public Action<IController> onDisable { get; set; }
        public Action<IController> onDestroy { get; set; }
        public Action<IManager> onRegisterManager { get; set; }
        public Action<IManager> onUnregisterManager { get; set; }

        public IManager Manager => _manager;
        private IManager _manager;
        

        public virtual async void InitAsync()
        {
            onInitialize?.Invoke(this);
            await Task.Yield();
            Start();
            await Task.Yield();
            await Task.Yield();
            LateStart();
        }

        public void RegisterManager(IManager manager)
        {
            _manager = manager;
            onRegisterManager?.Invoke(_manager);
        }

        public void UnregisterManager(IManager manager)
        {
            _manager = null;
            if (manager == null)
            {
                return;
            }
            onRegisterManager?.Invoke(manager);
        }

        public virtual void OnEnable()
        {
            onEnable?.Invoke(this);
        }

        public virtual void OnDisable()
        {
            onDisable?.Invoke(this);
        }

        public virtual void OnDestroy()
        {
            onDestroy?.Invoke(this);
        }

        public virtual void Start()
        {
            
        }

        public virtual void LateStart()
        {
            
        }

        public virtual void Update()
        {

        }

        public virtual void Destroy()
        {
            if (Application.isPlaying)
            {
                OnDestroy();
            }
        }
    }
}