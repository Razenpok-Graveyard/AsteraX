using AsteraX.Application;
using Asteroids;
using UnityEngine;
using VContainer;

namespace Player
{
    public class PlayerShipCollisionController : MonoBehaviour
    {
        private CommandHandler _commandHandler;
        private GameField _gameField;
        
        [Inject]
        public void Construct(CommandHandler commandHandler, GameField gameField)
        {
            _commandHandler = commandHandler;
            _gameField = gameField;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var asteroid = other.GetComponent<Asteroid>();
            if (asteroid != null)
            {
                _gameField.Destroy(asteroid);
                _gameField.DestroyPlayerShip();
                _commandHandler.Execute();
            }
        }

        public class CommandHandler
        {
            private readonly IGameSessionRepository _gameSessionRepository;

            public CommandHandler(IGameSessionRepository gameSessionRepository)
            {
                _gameSessionRepository = gameSessionRepository;
            }

            public void Execute()
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                gameSession.KillPlayer();
                DomainEvents.DispatchEvents(gameSession);
            }
        }
    }
}