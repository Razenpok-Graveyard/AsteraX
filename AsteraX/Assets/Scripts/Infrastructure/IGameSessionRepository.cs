using AsteraX.Domain.GameSession;

namespace AsteraX.Infrastructure
{
    public interface IGameSessionRepository
    {
        GameSession GetCurrentSession();
    }
}