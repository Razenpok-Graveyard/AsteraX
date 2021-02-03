using System;
using AsteraX.Mediator.Assets.Scripts;
using JetBrains.Annotations;
using VContainer;

namespace AsteraX.Infrastructure
{
    public class Mediator : ISender
    {
        private readonly IObjectResolver _objectResolver;

        public Mediator(IObjectResolver objectResolver) => _objectResolver = objectResolver;

        public void Send<TRequest>(TRequest request)
        {
            var handler = GetHandler<IRequestHandler<TRequest>>();
            handler.Handle(request);
        }

        [NotNull]
        private THandler GetHandler<THandler>()
        {
            THandler handler;

            try
            {
                handler = _objectResolver.Resolve<THandler>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error constructing handler for request of type {typeof(THandler)}. Register your handlers with the container.", e);
            }

            if (handler == null)
            {
                throw new InvalidOperationException($"Handler was not found for request of type {typeof(THandler)}. Register your handlers with the container.");
            }

            return handler;
        }
    }
}