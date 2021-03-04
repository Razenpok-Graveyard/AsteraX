using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure
{
    public interface IGameSessionRepository
    {
        GameSession Get();
        void Save();
    }
}