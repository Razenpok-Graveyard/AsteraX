using AsteraX.Domain.Game;
using Common.Domain;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionRepository : IGameSessionRepository, IGameSessionObservableModelRepository
    {
        private readonly GameSession _gameSession;
        private readonly GameSessionObservableModel _observableModel = new GameSessionObservableModel();

        public GameSessionRepository(GameSessionSettings settings)
        {
            _gameSession = new GameSession(settings.InitialJumps);
            _observableModel.Update(_gameSession);
        }

        public GameSessionRepository()
            : this(new GameSessionSettings())
        {
        }

        public GameSession Get()
        {
            return _gameSession;
        }

        public IGameSessionObservableModel GetObservableModel()
        {
            return _observableModel;
        }

        public void Save()
        {
            _observableModel.Update(_gameSession);
            DomainEventBus.DispatchEvents(_gameSession);
        }
    }
}