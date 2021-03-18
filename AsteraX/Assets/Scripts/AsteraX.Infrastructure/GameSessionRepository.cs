using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure
{
    public class GameSessionRepository
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

        public GameSession Get() => _gameSession;

        public GameSessionObservable GetObservable() => _observable;

        public void Save()
        {
            _observable.Update(_gameSession);
        }
    }
}