using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.SharedKernel;
using AsteraX.Infrastructure;
using Cysharp.Threading.Tasks;
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
                var command = new Command { Asteroid = asteroid };
                _commandHandler.Handle(command).Forget();
            }
        }

        public class Command : IRequest
        {
            public Asteroid Asteroid { get; set; }
        }

        public class CommandHandler : RequestHandler<Command>
        {
            private readonly IGameSessionRepository _gameSessionRepository;
            private readonly IGameField _gameField;

            public CommandHandler(IGameSessionRepository gameSessionRepository, IGameField gameField)
            {
                _gameSessionRepository = gameSessionRepository;
                _gameField = gameField;
            }

            protected override void HandleCore(Command command)
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                gameSession.KillPlayer();
                DomainEventBus.DispatchEvents(gameSession);
                _gameField.Destroy(command.Asteroid);
                _gameField.DestroyPlayerShip();
            }
        }
    }
}