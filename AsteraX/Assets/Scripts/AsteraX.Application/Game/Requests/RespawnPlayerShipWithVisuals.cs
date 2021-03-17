using System;
using Common.Application;

namespace AsteraX.Application.Game.Requests
{
    public class RespawnPlayerShipWithVisuals : IAsyncRequest
    {
        public TimeSpan Delay { get; set; }
    }
}