using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    [Serializable]
    public abstract class Manager<T> : IManager where T : GameService
    {
        public T Service => _service;
        [SerializeField] private T _service;
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onEnable { get; set; }
        public Action<IController> onDisable { get; set; }
        public Action<IController> onDestroy { get; set; }
        
        public Action<IManager> onAskForDisposal { get; set; }
        public List<Scene> Scenes => _scenes;

        private List<Scene> _scenes = new List<Scene>();

        public bool IsEnabled => _isEnabled;

        [HideInInspector] [SerializeField] protected bool _isEnabled = true;

        public virtual async void InitAsync()
        {
            RegisterService();
            if (_service == null || !_service.IsEnabled)
            {
                SetEnabled(false);
                return;
            }
            
            onInitialize?.Invoke(this);
            SetEnabled(_isEnabled);
            await Task.Yield();
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

        private void OnServiceWasEnabled(IController service)
        {
            SetEnabled(true);
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
                OnEnable();
                return;
            }
            OnDisable();
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

        public virtual void OnEnable()
        {
            if (_service == null)
            {
                RegisterService();
            }
            SceneManager.sceneUnloaded += OnSceneUnloaded;
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

        public virtual void Update()
        {
        }

        public virtual void Destroy()
        {
            if (_service == null)
            {
                return;
            }
            
            _service.UnregisterManager(this);
        }
    }
}