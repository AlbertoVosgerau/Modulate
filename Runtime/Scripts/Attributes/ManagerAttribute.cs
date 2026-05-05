using System;

namespace DandyDino.Modulate
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ManagerAttribute : Attribute
    {
        public bool IsSingleton { get; }
        
        public ManagerAttribute(bool isSingleton = true) => IsSingleton = isSingleton;
    }
}