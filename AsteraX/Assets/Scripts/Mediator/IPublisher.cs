namespace AsteraX.Mediator
{
    public interface IPublisher
    {
        void Publish<TNotification>(TNotification notification);
    }
}