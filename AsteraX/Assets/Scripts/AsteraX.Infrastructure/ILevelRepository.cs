using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure
{
    public interface ILevelRepository
    {
        public Level Get(long id);
    }
}