using AsteraX.Domain;
using AsteraX.Infrastructure;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
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

        private QueryHandler _queryHandler;

        [Inject]
        public void Construct(QueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }
        
        private void Awake()
        {
            this.SubscribeToDomain<GameOverEvent>(FadeIn);
        }

        private async UniTask FadeIn()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            var model = await _queryHandler.Handle(new Query());
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

            [NotNull]
            protected override Model HandleCore(Query query)
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