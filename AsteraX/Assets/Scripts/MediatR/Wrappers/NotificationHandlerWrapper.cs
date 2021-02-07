using Cysharp.Threading.Tasks;

namespace MediatR.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public abstract class NotificationHandlerWrapper
    {
        public abstract UniTask Handle(INotification notification, CancellationToken cancellationToken, ServiceFactory serviceFactory,
                                    Func<IEnumerable<Func<INotification, CancellationToken, UniTask>>, INotification, CancellationToken, UniTask> publish);
    }

    public class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
        where TNotification : INotification
    {
        public override UniTask Handle(INotification notification, CancellationToken cancellationToken, ServiceFactory serviceFactory,
                                    Func<IEnumerable<Func<INotification, CancellationToken, UniTask>>, INotification, CancellationToken, UniTask> publish)
        {
            var handlers = Enumerable.Select(serviceFactory
                    .GetInstances<INotificationHandler<TNotification>>(), x => new Func<INotification, CancellationToken, UniTask>((theNotification, theToken) => x.Handle((TNotification)theNotification, theToken)));

            return publish(handlers, notification, cancellationToken);
        }
    }
}