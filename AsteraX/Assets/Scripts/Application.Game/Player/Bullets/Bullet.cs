using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.Game.Commands;
using AsteraX.Application.SharedKernel;
using AsteraX.Infrastructure;
using Cysharp.Threading.Tasks;
using UniTaskPubSub;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.Game.Player.Bullets
{
    public class Bullet : MonoBehaviour
    {
        private float _speed;
        private float _lifetime;
        private float _passedLifetime;

        private CommandHandler _commandHandler;

        [Inject]
        public void Construct(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public void Initialize(float speed, float lifetime)
        {
            _speed = speed;
            _lifetime = lifetime;
        }

        private void Update()
        {
            _passedLifetime += Time.deltaTime;
            if (_passedLifetime >= _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            var command = new TranslateGameFieldObjectCommand(
                gameObject,
                Vector3.forward * Time.deltaTime * _speed);
            AsyncMessageBus.Default.Publish(command);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Asteroid>() != null)
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
                _commandHandler.Handle(new Command()).Forget();
            }
        }

        public class Command : IRequest { }

        public class CommandHandler : RequestHandler<Command>
        {
            private readonly IGameSessionRepository _gameSessionRepository;

            public CommandHandler(IGameSessionRepository gameSessionRepository)
            {
                _gameSessionRepository = gameSessionRepository;
            }

            protected override void HandleCore(Command request)
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                gameSession.Score += 100;
            }
        }
    }
}