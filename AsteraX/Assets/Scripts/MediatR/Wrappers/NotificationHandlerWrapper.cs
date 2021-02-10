using Cysharp.Threading.Tasks;

namespace MediatR.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public delegate UniTask PublishDelegate(
        IEnumerable<Func<INotification, CancellationToken, UniTask>> allHandlers,
        INotification notification,
        CancellationToken cancellationToken);
    
    public abstract class NotificationHandlerWrapper
    {
        public abstract UniTask Handle(
            INotification notification,
            CancellationToken cancellationToken,
            ServiceFactory serviceFactory,
            PublishDelegate publish);
    }

    public class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
        where TNotification : INotification
    {
        public override UniTask Handle(
            INotification notification,
            CancellationToken cancellationToken,
            ServiceFactory serviceFactory,
            PublishDelegate publish)
        {
            var handlers = serviceFactory
                .GetInstances<INotificationHandler<TNotification>>()
                .Select(handler => 
                    new Func<INotification, CancellationToken, UniTask>(
                        (n, t) => handler.Handle((TNotification)n, t)));

            return publish(handlers, notification, cancellationToken);
        }
    }
}