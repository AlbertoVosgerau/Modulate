using System;

namespace DandyDino.Modulate
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute
    {
        public ProvideAttribute(){}
    }
}