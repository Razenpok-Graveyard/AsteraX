using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Common.Application
{
    public interface IBaseRequestHandler
    {
    }

    public interface IRequestHandler<in TRequest, out TResponse> : IBaseRequestHandler
        where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
        where TRequest : IRequest<Unit>
    {
    }

    public interface IAsyncRequestHandler<in TRequest, TResponse> : IBaseRequestHandler
        where TRequest : IRequest<TResponse>
    {
        UniTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    public interface IAsyncRequestHandler<in TRequest> : IAsyncRequestHandler<TRequest, Unit>
        where TRequest : IRequest<Unit>
    {
    }

    public abstract class AsyncRequestHandler<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        UniTask<TResponse> IAsyncRequestHandler<TRequest, TResponse>.Handle(TRequest request,
            CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }

        protected abstract UniTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public abstract class AsyncRequestHandler<TRequest> : IAsyncRequestHandler<TRequest>
        where TRequest : IRequest
    {
        async UniTask<Unit> IAsyncRequestHandler<TRequest, Unit>.Handle(TRequest request,
            CancellationToken cancellationToken)
        {
            await Handle(request, cancellationToken);
            return Unit.Default;
        }

        protected abstract UniTask Handle(TRequest request, CancellationToken cancellationToken);
    }

    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        TResponse IRequestHandler<TRequest, TResponse>.Handle(TRequest request)
        {
            return Handle(request);
        }

        protected abstract TResponse Handle(TRequest request);
    }

    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        Unit IRequestHandler<TRequest, Unit>.Handle(TRequest request)
        {
            Handle(request);
            return Unit.Default;
        }

        protected abstract void Handle(TRequest request);
    }
}