using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Application.Unity
{
    public abstract class OutputRequestHandler<TRequest> : MonoBehaviour
        where TRequest : IRequest
    {
        protected virtual void Awake()
        {
            Subscribe();
        }

        protected virtual void Subscribe()
        {
            this.RegisterHandler<TRequest>(Handle);
        }

        protected abstract void Handle([NotNull] TRequest task);
    }

    public abstract class AsyncOutputRequestHandler<TRequest> : MonoBehaviour
        where TRequest : IAsyncRequest
    {
        protected virtual void Awake()
        {
            Subscribe();
        }

        protected virtual void Subscribe()
        {
            this.RegisterHandler<TRequest>(Handle);
        }

        protected abstract UniTask Handle([NotNull] TRequest task, CancellationToken ct);
    }
}