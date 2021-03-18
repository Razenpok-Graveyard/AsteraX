using Common.Application;

namespace AsteraX.Application.Game.Notifications
{
    public class HighScoreUpdated : INotification
    {
        public int Score { get; set; }
    }
}