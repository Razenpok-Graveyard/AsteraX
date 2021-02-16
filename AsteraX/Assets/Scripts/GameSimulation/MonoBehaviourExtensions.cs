using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UniTaskPubSub;
using UnityEngine;

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
}