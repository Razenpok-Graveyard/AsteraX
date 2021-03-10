using System;
using System.Threading;
using Common.Domain;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Common.Application.Unity
{
    public static class MonoBehaviourExtensions
    {
        public static IDisposable Subscribe<T>(
            this MonoBehaviour monoBehaviour,
            Func<T, CancellationToken, UniTask> action)
            where T : IAsyncApplicationTask
        {
            return ApplicationTaskDispatcher.Subscriber
                .Subscribe(action, monoBehaviour.GetCancellationTokenOnDestroy())
                .AddTo(monoBehaviour);
        }

        public static IDisposable Subscribe<T>(
            this MonoBehaviour monoBehaviour,
            Action<T> action)
            where T : IApplicationTask
        {
            return ApplicationTaskDispatcher.Subscriber
                .Subscribe(action)
                .AddTo(monoBehaviour);
        }

        public static IDisposable SubscribeToDomain<T>(
            this MonoBehaviour monoBehaviour,
            Func<T, CancellationToken, UniTask> action)
            where T : IDomainEvent
        {
            return DomainEventBus
                .Subscribe<T>(e => action(e, monoBehaviour.GetCancellationTokenOnDestroy()).Forget())
                .AddTo(monoBehaviour);
        }

        public static IDisposable SubscribeToDomain<T>(
            this MonoBehaviour monoBehaviour,
            Func<T, UniTask> action)
            where T : IDomainEvent
        {
            return DomainEventBus
                .Subscribe<T>(e => action(e).Forget())
                .AddTo(monoBehaviour);
        }

        public static IDisposable SubscribeToDomain<T>(
            this MonoBehaviour monoBehaviour,
            Action<T> action)
            where T : IDomainEvent
        {
            return DomainEventBus.Subscribe(action).AddTo(monoBehaviour);
        }
    }
}