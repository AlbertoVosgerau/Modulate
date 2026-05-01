using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    [DisallowMultipleComponent, AddComponentMenu(""),  DefaultExecutionOrder(-2_000_000_000)]
    public sealed class Modulate : MonoBehaviour
    {
        private static bool _isInitialized;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Init()
        {
            if (_isInitialized || !Application.isPlaying)
            {
                return;
            }
            
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Additive);
            
            _isInitialized = true;
            
            GameObject newObject = new GameObject(StringLibrary.MODULATE_NAME);
            _main = newObject.AddComponent<Modulate>();
            DontDestroyOnLoad(newObject);
        }
        

        public static Modulate Main => _main;
        private static Modulate _main;

        [Inject] private IEnumerable<IManager<IView>> _managers;
        
        private void Start()
        {
            if (_managers == null)
            {
                return;
            }
            
            foreach (var manager in _managers)
            {
                Debug.Log($"MODULATE Manager: {manager.GetType().Name}");
            }
        }
    }
}