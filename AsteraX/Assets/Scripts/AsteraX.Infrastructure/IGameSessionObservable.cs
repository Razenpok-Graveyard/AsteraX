using System;

namespace AsteraX.Infrastructure
{
    public interface IGameSessionObservable
    {
        IObservable<int> Jumps { get; }
        IObservable<int> Score { get; }
    }
}