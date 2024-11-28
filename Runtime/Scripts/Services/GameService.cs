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

        public async void InitAsync()
        {
            if (_isInitialized)
            {
                return;
            }
            EventBus<OnInitializeServer>.Raise(new OnInitializeServer(this));
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
                EventBus<OnDisposeService>.Raise(new OnDisposeService(this));
            }
        }
    }
}