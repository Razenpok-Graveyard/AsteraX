using AsteraX.Domain.GameSession;

namespace AsteraX.Application
{
    public interface IGameSessionRepository
    {
        GameSession GetCurrentSession();
    }

    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly GameSession _gameSession = new GameSession(3);
        
        public GameSession GetCurrentSession()
        {
            return _gameSession;
        }
    }
}