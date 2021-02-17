using System;
using AsteraX.Domain;
using AsteraX.Infrastructure;
using Cysharp.Threading.Tasks;
using UniRx;
using UniTaskPubSub;
using UnityEngine;

namespace AsteraX.Application.Game
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
            Action<T> action)
        {
            return AsyncMessageBus.Default.Subscribe(action).AddTo(monoBehaviour);
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