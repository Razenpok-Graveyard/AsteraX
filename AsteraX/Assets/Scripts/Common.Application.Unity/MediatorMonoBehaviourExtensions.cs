using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Common.Application.Unity
{
    public static class MediatorMonoBehaviourExtensions
    {
        public static OutputMediator Mediator;
        
        public static IDisposable RegisterHandler<TRequest>(
            this MonoBehaviour monoBehaviour,
            Func<TRequest, CancellationToken, UniTask> action)
            where TRequest : IAsyncRequest
        {
            return Mediator
                .RegisterRequestHandler(action, monoBehaviour.GetCancellationTokenOnDestroy())
                .AddTo(monoBehaviour);
        }

        public static IDisposable RegisterHandler<TRequest>(
            this MonoBehaviour monoBehaviour,
            Action<TRequest> action)
            where TRequest : IRequest
        {
            return Mediator
                .RegisterRequestHandler(action)
                .AddTo(monoBehaviour);
        }
    }
}