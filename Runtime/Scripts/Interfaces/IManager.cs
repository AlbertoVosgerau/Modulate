using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    public interface IManager: IController, ITogglable
    {
        public Action<IManager> onEnable { get; set; }
        public Action<IManager> onDisable { get; set; }
        public List<Scene> Scenes { get;}
        public void LateUpdate();
        public void FixedUpdate();
        public void OnEnable();
        public void OnDisable();
        public void Update();
        public void RegisterScenes(Scene scene);
    }
}