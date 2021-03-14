using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure.Data
{
    public class LevelRepository : ILevelRepository
    {
        public Level GetLevel()
        {
            return new Level(1, 1, 1);
        }
    }
}