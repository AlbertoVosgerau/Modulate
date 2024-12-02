using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    [Serializable, DefaultExecutionOrder(-10)]
    public abstract class Manager<T> : IManager where T : GameService, new()
    {
        public Action<IController> onDispose { get; set; }
        public Action<IManager> onEnable { get; set;}
        public Action<IManager> onDisable { get; set; }
        public T Service => _service;
        [SerializeField] private T _service;

        public bool IsInitialized => _isInitialized;
        private bool _isInitialized = false;

        public List<Scene> Scenes => _scenes;

        private List<Scene> _scenes = new List<Scene>();

        public bool IsEnabled => _isEnabled;

        [HideInInspector] [SerializeField] protected bool _isEnabled = true;

        public async void InitAsync()
        {
            if (_isInitialized)
            {
                OnEnableInternal();
                return;
            }
            RegisterService();
            SetEnabled(_isEnabled);
            _isInitialized = true;
            Awake();
            await Task.Yield();
            Start();
            await Task.Yield();
            await Task.Yield();
            LateStart();
        }

        private void RegisterService()
        {
            _service = Modulate.Main.GetGameService<T>();
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
                Dispose();
            }
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

        public virtual void OnDispose()
        {
           
        }

        public virtual void Update()
        {
        }

        public void Dispose()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            onDispose?.Invoke(this);
            
            if (_service == null)
            {
                return;
            }
            
            _service.Dispose();
        }
    }
}