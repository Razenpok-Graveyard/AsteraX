using System;
using AsteraX.Domain.Game;
using UniRx;

namespace AsteraX.Infrastructure.Data
{
    public class GameSessionObservableModel : IGameSessionObservableModel
    {
        private readonly ReactiveProperty<int> _jumps = new ReactiveProperty<int>();
        public IObservable<int> Jumps => _jumps;

        private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>();
        public IObservable<int> Score => _score;

        private readonly ReactiveProperty<bool> _isPlayerAlive = new ReactiveProperty<bool>();
        public IObservable<bool> IsPlayerAlive => _isPlayerAlive;

        public void Update(GameSession gameSession)
        {
            _jumps.Value = gameSession.Jumps;
            _score.Value = gameSession.Score;
            _isPlayerAlive.Value = gameSession.IsPlayerAlive;
        }
    }
}