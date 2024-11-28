using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DandyDino.Modulate
{
    [Serializable, DefaultExecutionOrder(-9)]
    public abstract class GameService : IService
    {
        public bool IsInitialized => _isInitialized;
        private bool _isInitialized = false;
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onDestroy { get; set; }

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
        
        public virtual void OnDestroy()
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