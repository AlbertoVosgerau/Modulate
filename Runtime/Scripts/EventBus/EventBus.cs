using System;
using System.Collections.Generic;

namespace DandyDino.Modulate.Event
{
    public static class EventBus<T> where T : IEvent
    {
        static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();

        public static void Register(ref EventBinding<T> eventBinding, Action action)
        {
            eventBinding = new EventBinding<T>(action);
            Register(eventBinding);
        }
        
        public static void Register(ref EventBinding<T> eventBinding, Action<T> action)
        {
            eventBinding = new EventBinding<T>(action);
            Register(eventBinding);
        }

        private static void Register(EventBinding<T> binding) => bindings.Add(binding);

        public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

        public static void Raise(T @event)
        {
            foreach (var binding in bindings)
            {
                binding.OnEvent?.Invoke(@event);
                binding.OnEventNoArgs?.Invoke();
            }
        }

        static void Clear()
        {
            bindings.Clear();
        }
    }
}