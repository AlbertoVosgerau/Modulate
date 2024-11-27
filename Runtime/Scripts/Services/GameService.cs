using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DandyDino.Modulate
{
    [Serializable]
    public abstract class GameService : IService
    {
        private bool _isInitialized = false;
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onDestroy { get; set; }
        public Action<IManager> onRegisterManager { get; set; }
        public Action<IManager> onUnregisterManager { get; set; }

        public IManager Manager => _manager;
        private IManager _manager;
        

        public async void InitAsync()
        {
            if (_isInitialized)
            {
                return;
            }
            onInitialize?.Invoke(this);
            _isInitialized = true;
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
            onUnregisterManager?.Invoke(manager);
            Destroy();
        }

        public virtual void OnDestroy()
        {
            
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

        public void Destroy()
        {
            if (Application.isPlaying)
            {
                OnDestroy();
                onDestroy?.Invoke(this);
            }
        }
    }
}