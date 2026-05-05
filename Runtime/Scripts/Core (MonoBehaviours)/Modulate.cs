using System;
using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

namespace DandyDino.Modulate
{
    [DisallowMultipleComponent, AddComponentMenu(""),  DefaultExecutionOrder(-1000)]
    public sealed class Modulate : MonoBehaviour
    {
        private static bool _isInitialized;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Init()
        {
            if (_isInitialized || !Application.isPlaying)
            {
                return;
            }
            _isInitialized = true;
            
            GameObject newObject = new GameObject(StringLibrary.MODULATE_NAME);
            _main = newObject.AddComponent<Modulate>();
            //DontDestroyOnLoad(newObject);
        }
        

        public static Modulate Main => _main;
        private static Modulate _main;

        [Inject] private IEnumerable<IManager> _managers;
        
        private void Start()
        {
            if (_managers == null)
            {
                return;
            }
            
            foreach (var manager in _managers)
            {
                Debug.Log($"MODULATE Manager: {manager.GetType().Name}");
                manager.Start();
            }
            
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (_managers == null)
            {
                return;
            }
            
            foreach (var manager in _managers)
            {
                manager.Update();
            }
        }

        private void FixedUpdate()
        {
            if (_managers == null)
            {
                return;
            }
            
            foreach (var manager in _managers)
            {
                manager.FixedUpdate();
            }
        }
        
        private void LateUpdate()
        {
            if (_managers == null)
            {
                return;
            }

            foreach (var manager in _managers)
            {
                manager.LateUpdate();
            }
        }
    }
}