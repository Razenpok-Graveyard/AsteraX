using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace AsteraX.Application
{
    public interface IBaseRequestHandler { }
    
    public interface IRequestHandler<in TRequest, TResponse> : IBaseRequestHandler
        where TRequest : IRequest<TResponse>
    {
        UniTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
        where TRequest : IRequest<Unit>
    {
    }

    public abstract class AsyncRequestHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        public async UniTask<Unit> Handle(TRequest request, CancellationToken cancellationToken = default)
        {
            await HandleCore(request, cancellationToken);
            return Unit.Default;
        }

        protected abstract UniTask HandleCore(TRequest request, CancellationToken cancellationToken = default);
    }

    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public UniTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
            => UniTask.FromResult(HandleCore(request));

        protected abstract TResponse HandleCore(TRequest request);
    }

    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        public UniTask<Unit> Handle(TRequest request, CancellationToken cancellationToken = default)
        {
            HandleCore(request);
            return UniTask.FromResult(Unit.Default);
        }

        protected abstract void HandleCore(TRequest request);
    }
}