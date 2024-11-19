using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        
        public IManager Manager { get; }
        private IManager _manager;
        
        public bool IsEnabled => _isEnabled;
        [HideInInspector] [SerializeField] protected bool _isEnabled = true;

        public virtual async void Init()
        {
            onInitialize?.Invoke(this);
            await UniTask.DelayFrame(1);
            Start();
            await UniTask.DelayFrame(1);
            LateStart();
        }

        public void RegisterManager(IManager manager)
        {
            if (_manager != null)
            {
                Debug.LogWarning($"Tried to register Manager {manager.GetType().Name} but it's already registered");
                return;
            }

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

        public virtual void SetEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;
            if (!Application.isPlaying)
            {
                return;
            }
            if (isEnabled)
            {
                OnEnable();
                return;
            }
            OnDisable();
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