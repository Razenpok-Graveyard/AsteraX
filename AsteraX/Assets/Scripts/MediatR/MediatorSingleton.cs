using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MediatR
{
    public class MediatorSingleton : MonoBehaviour
    {
        private static MediatorSingleton _instance;

        public IMediator Mediator { get; set; }

        private void Awake() => DontDestroyOnLoad(_instance = this);

        public static UniTask<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            return _instance.Mediator.Send(request, cancellationToken);
        }

        public static UniTask<object> Send(object request, CancellationToken cancellationToken = default)
        {
            return _instance.Mediator.Send(request, cancellationToken);
        }

        public static UniTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return _instance.Mediator.Publish(notification, cancellationToken);
        }

        public static UniTask Publish(object notification, CancellationToken cancellationToken = default)
        {
            return _instance.Mediator.Publish(notification, cancellationToken);
        }

        public static void Instantiate(IMediator mediator)
        {
            var go = new GameObject("Mediator");
            go.AddComponent<MediatorSingleton>().Mediator = mediator;
        }
    }
}