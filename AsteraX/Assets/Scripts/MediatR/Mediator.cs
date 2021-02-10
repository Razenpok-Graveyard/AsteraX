using Cysharp.Threading.Tasks;

namespace MediatR
{
    using Wrappers;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Default mediator implementation relying on single- and multi instance delegates for resolving handlers.
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly ServiceFactory _serviceFactory;

        private static readonly ConcurrentDictionary<Type, RequestHandlerBase> _requestHandlers =
            new ConcurrentDictionary<Type, RequestHandlerBase>();

        private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> _notificationHandlers =
            new ConcurrentDictionary<Type, NotificationHandlerWrapper>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="serviceFactory">The single instance factory.</param>
        public Mediator(ServiceFactory serviceFactory)
            => _serviceFactory = serviceFactory;

        public UniTask<TResponse> Send<TResponse>(IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();

            var handler = (RequestHandlerWrapper<TResponse>) _requestHandlers.GetOrAdd(requestType, CreateWrapperInstance);

            return handler.Handle(request, cancellationToken, _serviceFactory);

            RequestHandlerBase CreateWrapperInstance(Type type)
            {
                var handlerType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(type, typeof(TResponse));
                var instance = Activator.CreateInstance(handlerType) ??
                       throw new InvalidOperationException($"Could not create wrapper type for {type}");
                return (RequestHandlerBase) instance;
            }
        }

        public UniTask<object> Send(object request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var handler = _requestHandlers.GetOrAdd(requestType,
                requestTypeKey =>
                {
                    var requestInterfaceType = requestTypeKey
                        .GetInterfaces()
                        .FirstOrDefault(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

                    if (requestInterfaceType is null)
                    {
                        throw new ArgumentException($"{requestTypeKey.Name} does not implement {nameof(IRequest)}",
                            nameof(request));
                    }

                    var responseType = requestInterfaceType.GetGenericArguments()[0];
                    var wrapperType =
                        typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestTypeKey, responseType);

                    return (RequestHandlerBase) (Activator.CreateInstance(wrapperType)
                                                 ?? throw new InvalidOperationException(
                                                     $"Could not create wrapper for type {wrapperType}"));
                });

            // call via dynamic dispatch to avoid calling through reflection for performance reasons
            return handler.Handle(request, cancellationToken, _serviceFactory);
        }

        public UniTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            return PublishNotification(notification, cancellationToken);
        }

        public UniTask Publish(object notification, CancellationToken cancellationToken = default)
        {
            switch (notification)
            {
                case null:
                    throw new ArgumentNullException(nameof(notification));
                case INotification instance:
                    return PublishNotification(instance, cancellationToken);
                default:
                    throw new ArgumentException($"{nameof(notification)} does not implement ${nameof(INotification)}");
            }
        }

        /// <summary>
        /// Override in a derived class to control how the tasks are awaited. By default the implementation is a foreach and await of each handler
        /// </summary>
        /// <param name="allHandlers">Enumerable of tasks representing invoking each notification handler</param>
        /// <param name="notification">The notification being published</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing invoking all handlers</returns>
        protected virtual UniTask PublishCore(
            IEnumerable<Func<INotification, CancellationToken, UniTask>> allHandlers,
            INotification notification,
            CancellationToken cancellationToken)
        {
            var tasks = allHandlers
                .Select(handler => handler(notification, cancellationToken));
            return UniTask.WhenAll(tasks);
        }

        private UniTask PublishNotification(INotification notification, CancellationToken cancellationToken = default)
        {
            NotificationHandlerWrapper CreateHandlerInstance(Type type)
            {
                var instance = Activator.CreateInstance(typeof(NotificationHandlerWrapperImpl<>).MakeGenericType(type));
                if (instance == null)
                {
                    throw new InvalidOperationException($"Could not create wrapper for type {type}");
                }

                return (NotificationHandlerWrapper) instance;
            }

            var notificationType = notification.GetType();
            var handler = _notificationHandlers.GetOrAdd(notificationType, CreateHandlerInstance);
            return handler.Handle(notification, cancellationToken, _serviceFactory, PublishCore);
        }
    }
}