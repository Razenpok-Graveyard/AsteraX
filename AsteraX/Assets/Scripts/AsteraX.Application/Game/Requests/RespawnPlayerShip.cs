using Razensoft.Mediator;

namespace AsteraX.Application.Game.Requests
{
    public class RespawnPlayerShip : IRequest
    {
        public bool IntoInitialPosition { get; set; }
    }
}