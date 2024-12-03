using System;

namespace DandyDino.Modulate
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class ProvideAttribute : Attribute
    {
        public ProvideAttribute(){}
    }
}