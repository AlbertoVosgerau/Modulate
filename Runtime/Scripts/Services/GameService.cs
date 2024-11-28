using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DandyDino.Modulate
{
    [Serializable, DefaultExecutionOrder(-9)]
    public abstract class GameService : IService
    {
        public bool IsPersistent => _isPersistent;
        protected bool _isPersistent = false;
        
        public bool IsInitialized => _isInitialized;
        private bool _isInitialized = false;
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onDispose { get; set; }

        public async void InitAsync()
        {
            if (_isInitialized)
            {
                return;
            }
            onInitialize?.Invoke(this);
            _isInitialized = true;
            Awake();
            await Task.Yield();
            Start();
            await Task.Yield();
            await Task.Yield();
            LateStart();
        }
        
        public virtual void OnDispose()
        {
            
        }

        public virtual void Awake()
        {
            
        }

        public virtual void Start()
        {
            
        }

        public virtual void LateStart()
        {
            
        }
        

        public void Dispose()
        {
            if (_isPersistent)
            {
                return;
            }
            
            if (Application.isPlaying)
            {
                Modulate.Main.DisposeGameService(this);
                OnDispose();
                onDispose?.Invoke(this);
            }
        }
    }
}