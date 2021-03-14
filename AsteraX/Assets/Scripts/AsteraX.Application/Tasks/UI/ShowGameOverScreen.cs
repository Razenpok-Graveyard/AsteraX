using AsteraX.Domain.Game;
using Common.Application;

namespace AsteraX.Application.Tasks.UI
{
    public class ShowGameOverScreen : IAsyncApplicationTask
    {
        public long Level { get; set; }
        public int Score { get; set; }

        public static ShowGameOverScreen Create(GameSession gameSession)
        {
            return new ShowGameOverScreen
            {
                Level = gameSession.Level.Id,
                Score = gameSession.Score
            };
        }
    }
}