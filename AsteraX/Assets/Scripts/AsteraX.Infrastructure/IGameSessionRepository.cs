using AsteraX.Domain;

namespace AsteraX.Infrastructure
{
    public interface IGameSessionRepository
    {
        GameSession GetCurrentSession();
    }
}