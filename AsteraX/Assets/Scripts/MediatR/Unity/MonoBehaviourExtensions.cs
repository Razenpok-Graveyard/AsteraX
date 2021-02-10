using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace MediatR.Unity
{
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public static class MonoBehaviourExtensions
    {
        public static void RegisterNotificationHandler<TNotification>(
            this Component component,
            Action<TNotification> handler)
            where TNotification : INotification
        {
            RegisterNotificationHandler(component.gameObject, handler);
        }

        public static void RegisterNotificationHandler<TNotification>(
            this GameObject gameObject,
            Action<TNotification> handler)
            where TNotification : INotification
        {
            UnityMediator.RegisterNotificationHandler(gameObject, handler);
        }

        public static void RegisterRequestHandler<TRequest>(this Component component, Action<TRequest> handler)
            where TRequest : IRequest
        {
            RegisterRequestHandler(component.gameObject, handler);
        }

        public static void RegisterRequestHandler<TRequest>(this GameObject gameObject, Action<TRequest> handler)
            where TRequest : IRequest
        {
            UnityMediator.RegisterRequestHandler(gameObject, handler);
        }
    }
}