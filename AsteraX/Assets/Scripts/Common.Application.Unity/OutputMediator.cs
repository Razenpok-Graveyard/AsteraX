using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniTaskPubSub;

namespace Common.Application.Unity
{
    public class OutputMediator : IOutputMediator
    {
        public static readonly OutputMediator Default = new OutputMediator(new AsyncMessageBus());

        private readonly AsyncMessageBus _asyncMessageBus;

        public OutputMediator(AsyncMessageBus asyncMessageBus)
        {
            _asyncMessageBus = asyncMessageBus;
        }

        public void Publish<TNotification>(TNotification notification) where TNotification : INotification
        {
            SubscriptionTracker<TNotification>.ThrowIfNoSubscribers();
            _asyncMessageBus.Publish(notification);
        }

        public void Send<TRequest>(TRequest request) where TRequest : IRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfNoSubscribers();
            _asyncMessageBus.Publish(request);
        }

        public UniTask AsyncSend<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IAsyncRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfNoSubscribers();
            return _asyncMessageBus.PublishAsync(request, cancellationToken);
        }

        public void ForgetSend<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IAsyncRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfNoSubscribers();
            _asyncMessageBus.Publish(request, cancellationToken);
        }
        
        public IDisposable RegisterNotificationHandler<TNotification>(Action<TNotification> handler)
            where TNotification : INotification
        {
            var subscription = _asyncMessageBus.Subscribe(handler);
            return SubscriptionTracker<TNotification>.Track(subscription);
        }
        
        public IDisposable RegisterRequestHandler<TRequest>(Action<TRequest> handler)
            where TRequest : IRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfHasSubscribers();
            var subscription = _asyncMessageBus.Subscribe(handler);
            return SubscriptionTracker<TRequest>.Track(subscription);
        }

        public IDisposable RegisterRequestHandler<TRequest>(
            Func<TRequest, CancellationToken, UniTask> handler,
            CancellationToken ct = default)
            where TRequest : IAsyncRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfHasSubscribers();
            var subscription = _asyncMessageBus.Subscribe<TRequest>(
                (msg, ct2) => Handle(handler, msg, ct, ct2)
            );
            return SubscriptionTracker<TRequest>.Track(subscription);
        }

        private static async UniTask Handle<TRequest>(
            Func<TRequest, CancellationToken, UniTask> handler,
            TRequest message,
            CancellationToken ct,
            CancellationToken ct2)
            where TRequest : IAsyncRequest
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, ct2);
            await handler(message, linkedCts.Token);
        }

        private class SubscriptionTracker<T> : IDisposable
        {
            private IDisposable _subscription;

            private SubscriptionTracker(IDisposable subscription)
            {
                _subscription = subscription;
            }

            public void Dispose()
            {
                SubscriptionCount--;
            }

            public static int SubscriptionCount { get; private set; }

            public static void ThrowIfNoSubscribers()
            {
                if (SubscriptionCount == 0)
                {
                    throw new InvalidOperationException(
                        $"No handlers registered for handling {typeof(T).FullName}"
                    );
                }
            }

            public static void ThrowIfHasSubscribers()
            {
                if (SubscriptionCount != 0)
                {
                    throw new InvalidOperationException(
                        $"Only one handler can be registered for handling {typeof(T).FullName}"
                    );
                }
            }

            public static IDisposable Track(IDisposable subscription)
            {
                SubscriptionCount++;
                return new SubscriptionTracker<T>(subscription);
            }
        }
    }
}