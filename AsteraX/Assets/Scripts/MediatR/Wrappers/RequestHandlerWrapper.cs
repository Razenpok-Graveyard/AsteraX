using Cysharp.Threading.Tasks;

namespace MediatR.Wrappers
{
    using System;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading;

    public abstract class RequestHandlerBase
    {
        public abstract UniTask<object> Handle(object request, CancellationToken cancellationToken,
            ServiceFactory serviceFactory);

        protected static THandler GetHandler<THandler>(ServiceFactory factory)
        {
            THandler handler;

            try
            {
                handler = factory.GetInstance<THandler>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error constructing handler for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.", e);
            }

            if (handler == null)
            {
                throw new InvalidOperationException($"Handler was not found for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.");
            }

            return handler;
        }
    }

    public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
    {
        public abstract UniTask<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken,
            ServiceFactory serviceFactory);
    }

    public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override async UniTask<object> Handle(object request, CancellationToken cancellationToken,
            ServiceFactory serviceFactory)
        {
            try
            {
                return await Handle((IRequest<TResponse>) request, cancellationToken, serviceFactory);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }
                throw;
            }
        }

        public override UniTask<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken,
            ServiceFactory serviceFactory)
        {
            UniTask<TResponse> Handler() => GetHandler<IRequestHandler<TRequest, TResponse>>(serviceFactory)
                .Handle((TRequest) request, cancellationToken);

            return serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>) Handler,
                    (next, pipeline) => () => pipeline.Handle((TRequest) request, cancellationToken, next))();
        }
    }
}
