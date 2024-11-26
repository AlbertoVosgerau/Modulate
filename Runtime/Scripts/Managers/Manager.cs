using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    [Serializable]
    public abstract class Manager<T> : IManager where T : GameService, new()
    {
        public T Service => _service;
        [SerializeField] private T _service;

        private bool _isInitialized = false;
        
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onEnable { get; set; }
        public Action<IController> onDisable { get; set; }
        public Action<IController> onDestroy { get; set; }
        
        public Action<IManager> onAskForDisposal { get; set; }
        public List<Scene> Scenes => _scenes;

        private List<Scene> _scenes = new List<Scene>();

        public bool IsEnabled => _isEnabled;

        [HideInInspector] [SerializeField] protected bool _isEnabled = true;

        protected Manager()
        {
        }

        public virtual async void InitAsync()
        {
            if (_isInitialized)
            {
                OnEnableInternal();
                return;
            }
            RegisterService();
            onInitialize?.Invoke(this);
            SetEnabled(_isEnabled);
            _isInitialized = true;
            Start();
            await Task.Yield();
            await Task.Yield();
            LateStart();
        }

        private void RegisterService()
        {
            _service = Modulate.Main.GetService<T>();
            if (_service != null)
            {
                _service.RegisterManager(this);
            }
        }

        public void RegisterScenes(Scene scene)
        {
            if (_scenes.Contains(scene))
            {
                return;
            }

            _scenes.Add(scene);
        }

        public void SetEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;
            if (!Application.isPlaying)
            {
                return;
            }
            if (isEnabled)
            {
                OnEnableInternal();
                return;
            }
            OnDisableInternal();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (_scenes.Contains(scene))
            {
                _scenes.Remove(scene);
            }

            if (_scenes.Count == 0)
            {
                Debug.Log($"{GetType().Name} is not needed anymore and has been disposed.");
                onAskForDisposal?.Invoke(this);
            }
        }

        public virtual void Start()
        {
        }

        public virtual void LateStart()
        {
        }

        private void OnEnableInternal()
        {
            if (_service == null)
            {
                RegisterService();
            }
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            onEnable?.Invoke(this);
            OnEnable();
        }

        public virtual void LateUpdate()
        {
            
        }

        public virtual void FixedUpdate()
        {
            
        }

        public virtual void OnEnable()
        {
            
        }

        private void OnDisableInternal()
        {
            onDisable?.Invoke(this);
            OnDisable();
        }

        public virtual void OnDisable()
        {
            
        }

        public virtual void OnDestroy()
        {
           
        }

        public virtual void Update()
        {
        }

        public void Destroy()
        {
            if (_service == null)
            {
                return;
            }
            
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            _service.UnregisterManager(this);
            onDestroy?.Invoke(this);
        }
    }
}