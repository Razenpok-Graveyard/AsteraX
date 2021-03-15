using Common.Application;

namespace AsteraX.Application.Tasks.Game
{
    public class RespawnPlayerShip : IApplicationTask
    {
        public bool IntoInitialPosition { get; set; }
    }
}