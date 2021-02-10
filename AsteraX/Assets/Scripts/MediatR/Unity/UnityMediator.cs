using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MediatR.Unity
{
    public static class UnityMediator
    {
        private static readonly Dictionary<Type, List<object>> _requestHandlers
            = new Dictionary<Type, List<object>>();

        private static readonly Dictionary<Type, List<object>> _notificationHandlers
            = new Dictionary<Type, List<object>>();

        private static bool _isQuitting;

        static UnityMediator()
        {
            Application.quitting += () => _isQuitting = true;
        }

        public static IMediator SecondaryMediator { get; set; }

        public static void RegisterRequestHandler<TRequest>(GameObject gameObject, Action<TRequest> handler)
            where TRequest : IRequest
        {
            var requestHandler = new UnityRequestHandler<TRequest>(gameObject, handler);
            AddRequestHandler(typeof(TRequest), requestHandler);
        }

        public static void RegisterNotificationHandler<TNotification>(GameObject gameObject,
            Action<TNotification> handler)
            where TNotification : INotification
        {
            var requestHandler = new UnityNotificationHandler<TNotification>(gameObject, handler);
            AddNotificationHandler(typeof(TNotification), requestHandler);
        }

        private static void AddRequestHandler(Type type, object handler)
        {
            if (!_requestHandlers.TryGetValue(type, out var handlers))
            {
                handlers = new List<object>();
                _requestHandlers.Add(type, handlers);
            }

            handlers.Add(handler);
        }

        private static void AddNotificationHandler(Type type, object handler)
        {
            if (!_notificationHandlers.TryGetValue(type, out var handlers))
            {
                handlers = new List<object>();
                _notificationHandlers.Add(type, handlers);
            }

            handlers.Add(handler);
        }

        public static UniTask<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            if (_isQuitting)
            {
                return new UniTask<TResponse>();
            }

            var requestType = request.GetType();
            if (_requestHandlers.TryGetValue(requestType, out var handlers))
            {
                var aliveHandlers = new List<object>();
                BaseRequestHandler<TResponse> activeHandler = null;

                foreach (BaseRequestHandler<TResponse> handler in handlers)
                {
                    if (handler.IsAlive)
                    {
                        aliveHandlers.Add(handler);
                    }

                    if (activeHandler == null && handler.IsActive)
                    {
                        activeHandler = handler;
                    }
                }

                _requestHandlers[requestType] = aliveHandlers;

                if (activeHandler != null)
                {
                    return activeHandler.Handle(request, cancellationToken);
                }
            }

            if (SecondaryMediator == null)
            {
                throw new Exception("No active handler found in UnityMediator and secondary Mediator is not present");
            }

            return SecondaryMediator.Send(request, cancellationToken);
        }

        public static UniTask Publish(
            INotification notification,
            CancellationToken cancellationToken = default)
        {
            if (_isQuitting)
            {
                return new UniTask();
            }

            var notificationType = notification.GetType();
            var activeHandlers = new List<BaseNotificationHandler>();
            if (_notificationHandlers.TryGetValue(notificationType, out var handlers))
            {
                var aliveHandlers = new List<object>();

                foreach (BaseNotificationHandler handler in handlers)
                {
                    if (handler.IsAlive)
                    {
                        aliveHandlers.Add(handler);
                    }

                    if (handler.IsActive)
                    {
                        activeHandlers.Add(handler);
                    }
                }

                _notificationHandlers[notificationType] = aliveHandlers;
            }

            if (!activeHandlers.Any())
            {
                return SecondaryMediator.Publish(notification, cancellationToken);
            }

            var tasks = new List<UniTask>();
            foreach (var activeHandler in activeHandlers)
            {
                tasks.Add(activeHandler.Handle(notification, cancellationToken));
            }

            tasks.Add(SecondaryMediator.Publish(notification, cancellationToken));
            return UniTask.WhenAll(tasks);
        }

        private abstract class BaseHandler
        {
            private readonly GameObject _gameObject;

            protected BaseHandler(GameObject gameObject)
            {
                _gameObject = gameObject;
            }

            public bool IsActive => IsAlive && _gameObject.activeInHierarchy;

            public bool IsAlive => _gameObject != null;
        }

        private abstract class BaseRequestHandler<TResponse> : BaseHandler
        {
            protected BaseRequestHandler(GameObject gameObject) : base(gameObject) { }

            public abstract UniTask<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken);
        }

        private class UnityRequestHandler<TRequest> : BaseRequestHandler<Unit>
            where TRequest : IRequest<Unit>
        {
            private readonly Action<TRequest> _handler;

            public UnityRequestHandler(GameObject gameObject, Action<TRequest> handler) : base(gameObject)
            {
                _handler = handler;
            }

            public override UniTask<Unit> Handle(IRequest<Unit> request, CancellationToken cancellationToken)
            {
                _handler((TRequest) request);
                return Unit.Task;
            }
        }

        private abstract class BaseNotificationHandler : BaseHandler
        {
            protected BaseNotificationHandler(GameObject gameObject) : base(gameObject) { }

            public abstract UniTask Handle(INotification notification, CancellationToken cancellationToken);
        }

        private class UnityNotificationHandler<TNotification> : BaseNotificationHandler
            where TNotification : INotification
        {
            private readonly Action<TNotification> _handler;

            public UnityNotificationHandler(GameObject gameObject, Action<TNotification> handler) : base(gameObject)
            {
                _handler = handler;
            }

            public override UniTask Handle(INotification notification, CancellationToken cancellationToken)
            {
                _handler((TNotification) notification);
                return UniTask.CompletedTask;
            }
        }
    }
}