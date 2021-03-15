using System;
using Common.Application;

namespace AsteraX.Application.Tasks.Game
{
    public class RespawnPlayerShipWithVisuals : IAsyncApplicationTask
    {
        public TimeSpan Delay { get; set; }
    }
}