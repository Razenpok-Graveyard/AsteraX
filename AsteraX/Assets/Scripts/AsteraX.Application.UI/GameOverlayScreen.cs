using System.Threading;
using AsteraX.Infrastructure;
using Common.Application;
using Cysharp.Threading.Tasks;
using TMPro;
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
        public void Construct(QueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        private void Start()
        {
            UpdateValuesAsync(this.GetCancellationTokenOnDestroy())
                .SuppressCancellationThrow()
                .Forget();
        }

        private async UniTask UpdateValuesAsync(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                var model = _queryHandler.Handle(new Query());
                _jumps.text = $"{model.Jumps} Jumps";
                _score.text = $"{model.Score:C0}";
                await UniTask.Yield();
            }
        }

        public class Model
        {
            public int Jumps { get; set; }
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

            protected override Model Handle(Query request)
            {
                var gameSession = _gameSessionRepository.GetCurrentSession();
                return new Model
                {
                    Jumps = gameSession.Jumps,
                    Score = gameSession.Score
                };
            }
        }
    }
}