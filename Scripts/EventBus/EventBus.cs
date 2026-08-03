using System;
using System.Collections.Generic;

namespace tora.eventbus
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            _handlers.TryGetValue(type, out var existing);
            _handlers[type] = Delegate.Combine(existing, handler);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var existing))
            {
                return;
            }

            var combined = Delegate.Remove(existing, handler);
            if (combined == null)
            {
                _handlers.Remove(type);
            }
            else
            {
                _handlers[type] = combined;
            }
        }

        public static void Publish<T>(T message)
        {
            if (_handlers.TryGetValue(typeof(T), out var existing) && existing is Action<T> action)
            {
                action.Invoke(message);
            }
        }

        public static void Clear<T>()
        {
            _handlers.Remove(typeof(T));
        }
    }
}
