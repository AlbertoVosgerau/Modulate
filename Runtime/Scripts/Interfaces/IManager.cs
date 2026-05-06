using System;

namespace DandyDino.Modulate
{
    public interface IManager: IDisposable
    {
        Type ViewType { get; }
        bool IsSingleton { get; }

        void Start();
        void PreUpdate();
        void Update();
        void LateUpdate();
        void FixedUpdate();
        
    }
}