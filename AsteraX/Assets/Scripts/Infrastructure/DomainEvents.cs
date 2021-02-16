using System;
using System.Collections.Generic;
using AsteraX.Domain;
using CSharpFunctionalExtensions;

namespace AsteraX.Application
{
    public static class DomainEvents
    {
        private static readonly Dictionary<Type, List<Delegate>> Handlers
            = new Dictionary<Type, List<Delegate>>();

        private static readonly List<Action<object>> GeneralHandlers
            = new List<Action<object>>();

        public static void RegisterGeneralHandler(Action<object> eventHandler)
        {
            GeneralHandlers.Add(eventHandler);
        }

        public static void Register<T>(Action<T> eventHandler) where T : IDomainEvent
        {
            var eventType = typeof(T);
            if (!Handlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<Delegate>();
                Handlers.Add(eventType, handlers);
            }

            handlers.Add(eventHandler);
        }

        public static void Dispatch<T>(T domainEvent) where T : IDomainEvent
        {
            if (Handlers.TryGetValue(typeof(T), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    var action = (Action<T>) handler;
                    action(domainEvent);
                }
            }

            foreach (var handler in GeneralHandlers)
            {
                handler(domainEvent);
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
    }
}