using System;

namespace AsteraX.Infrastructure
{
    public interface IGameSessionObservableModel
    {
        IObservable<int> Jumps { get; }
        IObservable<int> Score { get; }
        IObservable<bool> IsPlayerAlive { get; }
    }
}