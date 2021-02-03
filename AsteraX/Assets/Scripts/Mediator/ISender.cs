namespace AsteraX.Mediator.Assets.Scripts
{
    public interface ISender
    {
        void Send<TRequest>(TRequest request);
    }
}