using System.Collections.Generic;
using R3;
using Reflex.Attributes;
using UnityEngine;

namespace DandyDino.Modulate
{
    [DisallowMultipleComponent, AddComponentMenu(""), DefaultExecutionOrder(-1000)]
    public sealed class Modulate : MonoBehaviour
    {
        private static bool _isInitialized;
        private static Modulate _main;
        public static Modulate Main => _main;

        [Inject] private IEnumerable<IManager> _managers;

        private DisposableBag _disposable;
        private IManager[] _managersArray;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Init()
        {
            if (_isInitialized || !Application.isPlaying)
            {
                return;
            }
            _isInitialized = true;

            GameObject go = new GameObject(StringLibrary.MODULATE_NAME);
            _main = go.AddComponent<Modulate>();
        }

        private void Start()
        {
            if (_managers == null) return;
            
            _managersArray = System.Linq.Enumerable.ToArray(_managers);
            
            IManager[] arr = _managersArray;
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].Start();
            }
            
            Observable.EveryUpdate(UnityFrameProvider.PreUpdate)
                .Subscribe(this, static (_, self) => self.TickPreUpdate())
                .AddTo(ref _disposable);
            
            Observable.EveryUpdate(UnityFrameProvider.Update)
                .Subscribe(this, static (_, self) => self.TickUpdate())
                .AddTo(ref _disposable);

            Observable.EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Subscribe(this, static (_, self) => self.TickFixedUpdate())
                .AddTo(ref _disposable);

            Observable.EveryUpdate(UnityFrameProvider.PostLateUpdate)
                .Subscribe(this, static (_, self) => self.TickLateUpdate())
                .AddTo(ref _disposable);

            DontDestroyOnLoad(gameObject);
        }
        
        private void TickPreUpdate()
        {
            IManager[] arr = _managersArray;
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].PreUpdate();
            }
        }

        private void TickUpdate()
        {
            IManager[] arr = _managersArray;
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].Update();
            }
        }

        private void TickFixedUpdate()
        {
            IManager[] arr = _managersArray;
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].FixedUpdate();
            }
        }

        private void TickLateUpdate()
        {
            IManager[] arr = _managersArray;
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].LateUpdate();
            }
        }

        private void OnDestroy() => _disposable.Dispose();
    }
}