using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DandyDino.Modulate
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();
        
        public static event Action<T> OnEvent
        {
            add
            {
                EventBinding<T> binding = new EventBinding<T>(value);
                Register(binding, value);
            }
            remove
            {
                IEventBinding<T> bindingToRemove = bindings.FirstOrDefault(b => b.OnEvent == value);
                if (bindingToRemove != null)
                {
                    Unregister(bindingToRemove);
                }
            }
        }
        
        private static void Register(EventBinding<T> eventBinding, Action<T> action)
        {
            eventBinding = new EventBinding<T>(action);
            Register(eventBinding);
        }

        private static void Register(EventBinding<T> binding) => bindings.Add(binding);
        private static void Unregister(IEventBinding<T> binding) => bindings.Remove(binding);

        public static void Raise(T @event)
        {
            foreach (var binding in bindings)
            {
                binding.OnEvent?.Invoke(@event);
                binding.OnEventNoArgs?.Invoke();
            }
        }

        private static void Clear()
        {
            bindings.Clear();
        }
    }
}