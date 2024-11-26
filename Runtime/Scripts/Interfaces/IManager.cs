using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    public interface IManager: IController, ITogglable
    {
        public Action<IController> onEnable { get; set; }
        public Action<IController> onDisable { get; set; }
        public Action<IManager> onAskForDisposal { get; set; }
        public List<Scene> Scenes { get;}
        public void LateUpdate();
        public void FixedUpdate();
        public void OnEnable();
        public void OnDisable();
        public void Update();
        public void RegisterScenes(Scene scene);
    }
}