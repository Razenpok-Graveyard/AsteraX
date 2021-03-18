using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure
{
    public interface ILevelRepository
    {
        Level Get(long id);
    }
}