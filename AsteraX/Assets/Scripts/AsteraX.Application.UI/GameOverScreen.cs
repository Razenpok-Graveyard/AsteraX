using AsteraX.Application.Common;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using TMPro;
using UnityEngine;
using VContainer;

namespace AsteraX.Application.UI
{
    public class GameOverScreen : MonoBehaviour 
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _score;

        private IRequestHandler<Query, Model> _queryHandler;

        [Inject]
        public void Construct(QueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }
        
        private void Start()
        {
            this.SubscribeToDomain<GameOverEvent>(_ => FadeIn());
        }

        private void FadeIn()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            var model = _queryHandler.Handle(new Query());
            _level.text = $"Final level: {model.Level}";
            _score.text = $"Final score: {model.Score}";
        }

        public class Model
        {
            public int Level { get; set; }
            public int Score { get; set; }
        }
        
        public class Query : IRequest<Model> { }

        public class QueryHandler : RequestHandler<Query, Model>
        {
            private readonly IGameSessionRepository _gameSessionRepository;

            public QueryHandler(IGameSessionRepository gameSessionRepository)
            {
                _gameSessionRepository = gameSessionRepository;
            }

            protected override Model Handle(Query query)
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                return new Model
                {
                    Level = 0,
                    Score = gameSession.Score
                };
            }
        }
    }
}