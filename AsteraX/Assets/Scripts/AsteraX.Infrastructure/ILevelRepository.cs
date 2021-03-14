using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure
{
    public interface ILevelRepository
    {
        public Level GetLevel(long id);
    }
}