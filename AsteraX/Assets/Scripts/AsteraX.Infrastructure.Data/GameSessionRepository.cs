using AsteraX.Domain;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly GameSession _gameSession = new GameSession(3);
        
        public GameSession GetCurrentSession()
        {
            return _gameSession;
        }
    }
}