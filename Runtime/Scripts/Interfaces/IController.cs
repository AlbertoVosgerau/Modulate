using System;

namespace DandyDino.Modulate
{
    public interface IController : IDisposable
    {
        public Action<IController> onDispose { get; set; }
        public bool IsInitialized { get; }
        
        public void InitAsync();
        public void Awake();
        public void Start();
        public void LateStart();
        public void OnDispose();
    }
}