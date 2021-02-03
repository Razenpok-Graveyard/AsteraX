namespace AsteraX.Mediator.Assets.Scripts
{
    public interface IRequestHandler<in TRequest>
    {
        void Handle(TRequest command);
    }
}