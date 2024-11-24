using System;

namespace DandyDino.Modulate
{
    public interface IController
    {
        public Action<IController> onInitialize { get; set; }
        public Action<IController> onEnable { get; set; }
        public Action<IController> onDisable { get; set; }
        public Action<IController> onDestroy { get; set; }
        public void InitAsync();
        public void Start();
        public void LateStart();
        public void OnDestroy();
        public void Update();
        public void Destroy();
    }
}