using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Razensoft.Mediator;
using UniTaskPubSub;

namespace AsteraX.Application
{
    public class MessageBus : IMessageBus
    {
        private readonly AsyncMessageBus _asyncMessageBus;

        public MessageBus(AsyncMessageBus asyncMessageBus)
        {
            _asyncMessageBus = asyncMessageBus;
        }
        
        public void Publish<T>(T message)
        {
            _asyncMessageBus.Publish(message);
        }

        public UniTask PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        {
            return _asyncMessageBus.PublishAsync(message, cancellationToken);
        }

        public IDisposable Subscribe<T>(Func<T, CancellationToken, UniTask> action, CancellationToken cancellationToken = default)
        {
            return _asyncMessageBus.Subscribe(action, cancellationToken);
        }

        public IDisposable Subscribe<T>(Action<T> action)
        {
            return _asyncMessageBus.Subscribe(action);
        }
    }
}