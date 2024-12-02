using System;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance.transform.parent != null)
                    {
                        _instance.transform.SetParent(null);
                    }
                    
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                        
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
    }
}
