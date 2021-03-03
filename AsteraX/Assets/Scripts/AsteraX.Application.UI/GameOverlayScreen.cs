using System;
using AsteraX.Infrastructure;
using Common.Application;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.UI
{
    public class GameOverlayScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _jumps;
        [SerializeField] private TextMeshProUGUI _score;

        private IRequestHandler<Query, Model> _queryHandler;

        [Inject]
        public void Construct(IRequestHandler<Query, Model> queryHandler)
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

        public class QueryHandler : RequestHandler<Query, Model>
        {
            private readonly IGameSessionObservableModelRepository _gameSessionObservableModelRepository;

            public QueryHandler(IGameSessionObservableModelRepository gameSessionObservableModelRepository)
            {
                _gameSessionObservableModelRepository = gameSessionObservableModelRepository;
            }

            protected override Model Handle(Query request)
            {
                var readModel = _gameSessionObservableModelRepository.GetObservableModel();
                return new Model
                {
                    Jumps = readModel.Jumps,
                    Score = readModel.Score
                };
            }
        }
    }
}