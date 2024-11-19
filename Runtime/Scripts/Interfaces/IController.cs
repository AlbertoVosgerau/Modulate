using System;

namespace DandyDino.Modulate
{
    public interface IController : ITogglable
    {
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onEnable { get; set; }
        public Action<IController> onDisable { get; set; }
        public Action<IController> onDestroy { get; set; }
        public void Init();
        public void Start();
        public void LateStart();
        public void OnEnable();
        public void OnDisable();
        public void OnDestroy();
        public void Update();
        public void Destroy();
    }
}