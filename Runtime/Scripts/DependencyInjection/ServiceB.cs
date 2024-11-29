using UnityEngine;

namespace DandyDino.Modulate
{
    public class ServiceB
    {
        public void Initialize(string message = null)
        {
            Debug.Log($"Initialized Service B with message {message}");
        }
    }
}