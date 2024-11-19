using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    public interface IManager: IController
    {
        public Action<IManager> onAskForDisposal { get; set; }
        public List<Scene> Scenes { get;}
        public void RegisterScenes(Scene scene);
    }
}