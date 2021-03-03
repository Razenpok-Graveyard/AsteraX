using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionObservableModelRepository : IGameSessionObservableModelRepository
    {
        private readonly GameSessionObservableModel _observableModel = new GameSessionObservableModel();
        
        public IGameSessionObservableModel GetObservableModel()
        {
            return _observableModel;
        }

        public void Update(GameSession gameSession)
        {
            _observableModel.Update(gameSession);
        }
    }
}