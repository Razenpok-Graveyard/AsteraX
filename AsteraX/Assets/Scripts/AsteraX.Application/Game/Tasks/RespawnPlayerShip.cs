using Common.Application;

namespace AsteraX.Application.Game.Tasks
{
    public class RespawnPlayerShip : IApplicationTask
    {
        public bool IntoInitialPosition { get; set; }
    }
}