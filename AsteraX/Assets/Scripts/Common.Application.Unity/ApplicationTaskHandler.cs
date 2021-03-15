using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Application.Unity
{
    public abstract class ApplicationTaskHandler<T> : MonoBehaviour
        where T : IApplicationTask
    {
        protected virtual void Awake()
        {
            Subscribe();
        }

        protected virtual void Subscribe()
        {
            this.Subscribe<T>(Handle);
        }

        protected abstract void Handle([NotNull] T task);
    }

    public abstract class AsyncApplicationTaskHandler<T> : MonoBehaviour
        where T : IAsyncApplicationTask
    {
        protected virtual void Awake()
        {
            Subscribe();
        }

        protected virtual void Subscribe()
        {
            this.Subscribe<T>(Handle);
        }

        protected abstract UniTask Handle([NotNull] T task, CancellationToken ct);
    }
}