using AsteraX.Application.Game.Asteroids;
using AsteraX.Infrastructure;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class PlayerShipCollisionController : MonoBehaviour
    {
        private CommandHandler _commandHandler;
        
        [Inject]
        public void Construct(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var asteroid = other.GetComponent<Asteroid>();
            if (asteroid != null)
            {
                _commandHandler.Execute(asteroid);
            }
        }

        public class CommandHandler
        {
            private readonly IGameSessionRepository _gameSessionRepository;
            private readonly IGameField _gameField;

            public CommandHandler(IGameSessionRepository gameSessionRepository, IGameField gameField)
            {
                _gameSessionRepository = gameSessionRepository;
                _gameField = gameField;
            }

            public void Execute(Asteroid asteroid)
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                gameSession.KillPlayer();
                DomainEventBus.DispatchEvents(gameSession);
                _gameField.Destroy(asteroid);
                _gameField.DestroyPlayerShip();
            }
        }
    }
}