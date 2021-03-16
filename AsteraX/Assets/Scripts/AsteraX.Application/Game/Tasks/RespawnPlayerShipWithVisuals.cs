using System;
using Common.Application;

namespace AsteraX.Application.Game.Tasks
{
    public class RespawnPlayerShipWithVisuals : IAsyncApplicationTask
    {
        public TimeSpan Delay { get; set; }
    }
}