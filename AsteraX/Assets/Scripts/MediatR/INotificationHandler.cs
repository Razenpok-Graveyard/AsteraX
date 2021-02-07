using System.Threading;
using Cysharp.Threading.Tasks;

namespace MediatR
{
    /// <summary>
    /// Defines a handler for a notification
    /// </summary>
    /// <typeparam name="TNotification">The type of notification being handled</typeparam>
    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        /// <summary>
        /// Handles a notification
        /// </summary>
        /// <param name="notification">The notification</param>
        /// <param name="cancellationToken">Cancellation token</param>
        UniTask Handle(TNotification notification, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Wrapper class for a synchronous notification handler
    /// </summary>
    /// <typeparam name="TNotification">The notification type</typeparam>
    public abstract class NotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        UniTask INotificationHandler<TNotification>.Handle(TNotification notification, CancellationToken cancellationToken)
        {
            Handle(notification);

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Override in a derived class for the handler logic
        /// </summary>
        /// <param name="notification">Notification</param>
        protected abstract void Handle(TNotification notification);
    }
}