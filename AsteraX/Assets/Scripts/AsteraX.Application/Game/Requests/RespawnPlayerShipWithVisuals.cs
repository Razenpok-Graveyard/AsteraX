using System;
using Razensoft.Mediator;

namespace AsteraX.Application.Game.Requests
{
    public class RespawnPlayerShipWithVisuals : IAsyncRequest
    {
        public TimeSpan Delay { get; set; }
    }
}