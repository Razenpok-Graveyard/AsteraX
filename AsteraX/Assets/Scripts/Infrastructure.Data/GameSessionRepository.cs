using AsteraX.Domain.GameSession;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly GameSession _gameSession = new GameSession(1);
        
        public GameSession GetCurrentSession()
        {
            return _gameSession;
        }
    }
}