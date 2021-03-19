using Razensoft.Mediator;

namespace AsteraX.Application.Game.Requests
{
    public class DestroyAsteroid : IRequest
    {
        public long Id { get; set; }
    }
}