using System;
using Common.Application;

namespace AsteraX.Application.Tasks.Game
{
    public class RespawnPlayerShip : IAsyncApplicationTask
    {
        public TimeSpan Delay { get; set; }
        public bool SpawnEffects { get; set; }
    }
}