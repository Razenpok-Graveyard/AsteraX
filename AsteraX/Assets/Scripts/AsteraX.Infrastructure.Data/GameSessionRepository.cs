using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly GameSession _gameSession = new GameSession(3);
        private readonly GameSessionObservableModelRepository _observableModelRepository;

        public GameSessionRepository(GameSessionObservableModelRepository observableModelRepository)
        {
            _observableModelRepository = observableModelRepository;
            _observableModelRepository.Update(_gameSession);
        }

        public GameSession GetCurrentSession()
        {
            return _gameSession;
        }

        public void Commit()
        {
            DomainEventBus.DispatchEvents(_gameSession);
            _observableModelRepository.Update(_gameSession);
        }
    }
}