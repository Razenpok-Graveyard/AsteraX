using AsteraX.Domain.Game;
using Common.Domain;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionRepository : IGameSessionRepository, IGameSessionObservableRepository
    {
        private readonly GameSession _gameSession;
        private readonly GameSessionObservable _observable = new GameSessionObservable();

        public GameSessionRepository(GameSessionSettings settings)
        {
            _gameSession = new GameSession(settings.InitialJumps);
            _observable.Update(_gameSession);
        }

        public GameSessionRepository()
            : this(new GameSessionSettings())
        {
        }

        public GameSession Get()
        {
            return _gameSession;
        }

        IGameSessionObservable IGameSessionObservableRepository.Get()
        {
            return _observable;
        }

        public void Save()
        {
            _observable.Update(_gameSession);
            DomainEventBus.DispatchEvents(_gameSession);
        }
    }
}