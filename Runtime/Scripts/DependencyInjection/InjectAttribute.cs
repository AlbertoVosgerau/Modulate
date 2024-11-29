using System;

namespace DandyDino.Modulate
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute
    {
        public InjectAttribute(){}
    }
}