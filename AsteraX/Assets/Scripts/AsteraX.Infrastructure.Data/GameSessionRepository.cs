using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private GameSession _gameSession = new GameSession(3);
        
        public GameSession GetCurrentSession()
        {
            return _gameSession;
        }

        public void Save(GameSession gameSession)
        {
            _gameSession = gameSession;
            DomainEventBus.DispatchEvents(gameSession);
        }
    }
}