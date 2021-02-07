using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MediatR
{
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public static class MonoBehaviourExtensions
    {
        public static UniTask<TResponse> Send<TResponse>(
            this MonoBehaviour _,
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            return MediatorSingleton.Send(request, cancellationToken);
        }

        public static UniTask<object> Send(
            this MonoBehaviour _,
            object request,
            CancellationToken cancellationToken = default)
        {
            return MediatorSingleton.Send(request, cancellationToken);
        }

        public static UniTask Publish<TNotification>(
            this MonoBehaviour _,
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return MediatorSingleton.Publish(notification, cancellationToken);
        }

        public static UniTask Publish(
            this MonoBehaviour _,
            object notification,
            CancellationToken cancellationToken = default)
        {
            return MediatorSingleton.Publish(notification, cancellationToken);
        }
    }
}