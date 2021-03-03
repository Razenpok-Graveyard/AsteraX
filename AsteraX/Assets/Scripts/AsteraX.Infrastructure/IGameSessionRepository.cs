using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure
{
    public interface IGameSessionRepository
    {
        GameSession GetCurrentSession();
        void Commit();
    }
}