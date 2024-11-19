using System;
using System.Collections.Generic;

namespace DandyDino.Modulate
{
    public interface IService : IController
    {
        public Action<IManager> onRegisterManager { get; }
        public Action<IManager> onUnregisterManager { get; }
        
        public IManager Manager { get; }
        public void RegisterManager(IManager manager);
        public void UnregisterManager(IManager manager);
    }
}