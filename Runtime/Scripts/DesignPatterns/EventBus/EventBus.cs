using System;
using R3;

namespace DandyDino.Modulate
{
    public static class EventBus<T> where T : IEvent
    {
        private static Action<T> _withArg;
        private static Action _noArg;

        public static void Register(Action<T> handler) => _withArg += handler;
        public static void Unregister(Action<T> handler) => _withArg -= handler;

        public static void Register(Action handler) => _noArg += handler;
        public static void Unregister(Action handler) => _noArg -= handler;
        
        static EventBus()
        {
            EventBusUtils.RegisterBus(Clear);
        }

        public static void Raise(T @event)
        {
            Action<T> withArg = _withArg;
            Action noArg   = _noArg;
            withArg?.Invoke(@event);
            noArg?.Invoke();
        }
        
        public static Observable<T> AsObservable() => Observable.FromEvent<T>( h => _withArg += h, h => _withArg -= h);

        private static void Clear()
        {
            _withArg = null;
            _noArg   = null;
        }
    }
}