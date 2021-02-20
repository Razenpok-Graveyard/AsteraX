using System;
using System.Collections.Generic;
using AsteraX.Domain;

namespace AsteraX.Infrastructure
{
    public static class DomainEventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> Handlers
            = new Dictionary<Type, List<Delegate>>();

        public static IDisposable Subscribe<T>(Action<T> eventHandler) where T : IDomainEvent
        {
            var eventType = typeof(T);
            if (!Handlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<Delegate>();
                Handlers.Add(eventType, handlers);
            }

            handlers.Add(eventHandler);
            return new Subscription(handlers, eventHandler);
        }

        public static void Dispatch<T>(T domainEvent) where T : IDomainEvent
        {
            if (Handlers.TryGetValue(domainEvent.GetType(), out var handlers))
            {
                var args = new object[] { domainEvent };
                foreach (var handler in handlers)
                {
                    handler.DynamicInvoke(args);
                }
            }
        }

        public static void DispatchEvents(AggregateRoot aggregateRoot)
        {
            if (aggregateRoot == null)
                return;

            foreach (var domainEvent in aggregateRoot.DomainEvents)
            {
                Dispatch(domainEvent);
            }

            aggregateRoot.ClearEvents();
        }

        private class Subscription : IDisposable
        {
            private readonly List<Delegate> _handlers;
            private readonly Delegate _handler;

            public Subscription(List<Delegate> handlers, Delegate handler)
            {
                _handlers = handlers;
                _handler = handler;
            }

            public void Dispose()
            {
                _handlers.Remove(_handler);
            }
        }
    }
}