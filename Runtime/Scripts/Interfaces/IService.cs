using System;

namespace DandyDino.Modulate
{
    public interface IService : IController
    {
        public bool IsPersistent { get; }
    }
}