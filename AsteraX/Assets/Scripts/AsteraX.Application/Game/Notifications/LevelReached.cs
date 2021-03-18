using Common.Application;

namespace AsteraX.Application.Game.Notifications
{
    public class LevelReached : INotification
    {
        public long Id { get; set; }
    }
}