using System;

namespace DandyDino.Modulate
{
    public interface IController : IDisposable
    {
        public bool IsInitialized { get; }
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onDispose { get; set; }
        
        public void InitAsync();
        public void Awake();
        public void Start();
        public void LateStart();
        public void OnDispose();
    }
}