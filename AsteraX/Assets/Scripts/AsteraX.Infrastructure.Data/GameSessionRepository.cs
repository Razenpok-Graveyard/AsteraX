using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly GameSession _gameSession;
        private readonly GameSessionObservableModelRepository _observableModelRepository;

        public GameSessionRepository(
            GameSessionObservableModelRepository observableModelRepository,
            GameSessionSettings settings)
        {
            _observableModelRepository = observableModelRepository;
            _gameSession = new GameSession(settings.InitialJumps);
            _observableModelRepository.Update(_gameSession);
        }

        public GameSessionRepository(GameSessionObservableModelRepository observableModelRepository)
            : this(observableModelRepository, new GameSessionSettings())
        {
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