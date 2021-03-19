using System;
using AsteraX.Infrastructure;
using Razensoft.Mediator;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.UI.GameOverlay
{
    public class GameOverlayScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _jumps;
        [SerializeField] private TextMeshProUGUI _score;

        private IInputRequestHandler<Query, Model> _queryHandler;

        [Inject]
        public void Construct(IInputRequestHandler<Query, Model> queryHandler)
        {
            _queryHandler = queryHandler;
        }

        private void Start()
        {
            var model = _queryHandler.Handle(new Query());
            model.Jumps
                .Select(jumps => $"{jumps} Jumps")
                .SubscribeToText(_jumps)
                .AddTo(this);
            model.Score
                .Select(score => $"{score:C0}")
                .SubscribeToText(_score)
                .AddTo(this);
        }

        public class Model
        {
            public IObservable<int> Jumps { get; set; }
            public IObservable<int> Score { get; set; }
        }
        
        public class Query : IRequest<Model> { }

        public class QueryHandler : InputRequestHandler<Query, Model>
        {
            private readonly GameSessionRepository _observableRepository;

            public QueryHandler(GameSessionRepository observableRepository)
            {
                _observableRepository = observableRepository;
            }

            protected override Model Handle(Query request)
            {
                var gameSession = _observableRepository.GetObservable();
                return new Model
                {
                    Jumps = gameSession.Jumps,
                    Score = gameSession.Score
                };
            }
        }
    }
}