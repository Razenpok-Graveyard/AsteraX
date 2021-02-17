using System;
using AsteraX.Domain;
using AsteraX.Infrastructure;
using Cysharp.Threading.Tasks;
using UniRx;
using UniTaskPubSub;
using UnityEngine;

namespace AsteraX.Application.SharedKernel
{
    public static class MonoBehaviourExtensions
    {
        public static IDisposable Subscribe<T>(
            this MonoBehaviour monoBehaviour,
            Func<T, UniTask> action)
        {
            return AsyncMessageBus.Default.Subscribe(action).AddTo(monoBehaviour);
        }

        public static IDisposable Subscribe<T>(
            this MonoBehaviour monoBehaviour,
            Func<UniTask> action)
        {
            return AsyncMessageBus.Default.Subscribe<T>(_ => action()).AddTo(monoBehaviour);
        }

        public static IDisposable Subscribe<T>(
            this MonoBehaviour monoBehaviour,
            Action<T> action)
        {
            return AsyncMessageBus.Default.Subscribe(action).AddTo(monoBehaviour);
        }

        public static IDisposable Subscribe<T>(
            this MonoBehaviour monoBehaviour,
            Action action)
        {
            return AsyncMessageBus.Default.Subscribe<T>(_ => action()).AddTo(monoBehaviour);
        }

        public static IDisposable SubscribeToDomain<T>(
            this MonoBehaviour monoBehaviour,
            Func<T, UniTask> action)
            where T : IDomainEvent
        {
            return DomainEventBus.Subscribe<T>(e => action(e).Forget()).AddTo(monoBehaviour);
        }

        public static IDisposable SubscribeToDomain<T>(
            this MonoBehaviour monoBehaviour,
            Func<UniTask> action)
            where T : IDomainEvent
        {
            return DomainEventBus.Subscribe<T>(_ => action().Forget()).AddTo(monoBehaviour);
        }

        public static IDisposable SubscribeToDomain<T>(
            this MonoBehaviour monoBehaviour,
            Action<T> action)
            where T : IDomainEvent
        {
            return DomainEventBus.Subscribe(action).AddTo(monoBehaviour);
        }

        public static IDisposable SubscribeToDomain<T>(
            this MonoBehaviour monoBehaviour,
            Action action)
            where T : IDomainEvent
        {
            return DomainEventBus.Subscribe<T>(_ => action()).AddTo(monoBehaviour);
        }
    }
}