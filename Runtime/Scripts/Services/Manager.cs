using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    [Serializable]
    public abstract class Manager<T> : IManager where T : GameService
    {
        public T Service => _service;
        protected T _service;
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onEnable { get; set; }
        public Action<IController> onDisable { get; set; }
        public Action<IController> onDestroy { get; set; }
        
        public Action<IManager> onAskForDisposal { get; set; }
        public List<Scene> Scenes => _scenes;

        private List<Scene> _scenes = new List<Scene>();

        public bool IsEnabled => _isEnabled;

        [HideInInspector] [SerializeField] protected bool _isEnabled = true;

        public virtual async void Init()
        {
            _service = Modulate.Main.GetService<T>();
            onInitialize?.Invoke(this);
            SetEnabled(_isEnabled);
            await UniTask.DelayFrame(1);
            Start();
            await UniTask.DelayFrame(1);
            LateStart();
        }

        public void RegisterScenes(Scene scene)
        {
            if (_scenes.Contains(scene))
            {
                return;
            }
            Debug.Log($"Registered scene {scene.name} on {GetType().Name}");
            _scenes.Add(scene);
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
        }
    }
}