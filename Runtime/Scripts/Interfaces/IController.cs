using System;

namespace DandyDino.Modulate
{
    public interface IController
    {
        public bool IsInitialized { get; }
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onDestroy { get; set; }
        public void InitAsync();
        public void Awake();
        public void Start();
        public void LateStart();
        public void OnDestroy();
        public void Destroy();
    }
}