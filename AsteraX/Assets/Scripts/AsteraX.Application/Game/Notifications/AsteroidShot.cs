using Common.Application;

namespace AsteraX.Application.Game.Notifications
{
    public class AsteroidShot : INotification
    {
        public bool IsLuckyShot { get; set; }
    }
}