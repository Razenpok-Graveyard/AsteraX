using AsteraX.Application.Game;
using Common.Application;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AsteraX.Application.UI
{
    public class StartButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private IRequestHandler<Command> _commandHandler;

        [Inject]
        public void Construct(IRequestHandler<Command> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        private void Start()
        {
            gameObject.OnEnableAsObservable()
                .Subscribe(_ => _button.interactable = true)
                .AddTo(this);

            _button.OnClickAsObservable()
                .Subscribe(_ => _commandHandler.Handle(new Command()))
                .AddTo(this);
        }

        public class Command : IRequest { }

        public class CommandHandler : RequestHandler<Command>
        {
            private readonly IApplicationTaskPublisher _taskPublisher;

            public CommandHandler(IApplicationTaskPublisher taskPublisher)
            {
                _taskPublisher = taskPublisher;
            }
            
            protected override void Handle(Command command)
            {
                _taskPublisher.PublishTask(new CloseMainMenu());
                _taskPublisher.PublishAsyncTask(new StartNextLevel()).Forget();
            }
        }
    }
}