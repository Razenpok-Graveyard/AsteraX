using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure
{
    public class GameSessionRepository
    {
        private readonly GameSession _gameSession;
        private readonly GameSessionObservable _observable = new GameSessionObservable();

        public GameSessionRepository(SaveFile saveFile, GameSessionSettings settings)
        {
            var contents = saveFile.GetContents();
            _gameSession = new GameSession(settings.InitialJumps, contents.HighScore);
            _observable.Update(_gameSession);
        }

        public GameSessionRepository(SaveFile saveFile)
            : this(saveFile, new GameSessionSettings())
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