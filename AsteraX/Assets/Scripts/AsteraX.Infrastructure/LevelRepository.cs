using System;
using AsteraX.Domain.Game;

namespace AsteraX.Infrastructure
{
    public class LevelRepository : ILevelRepository
    {
        private readonly LevelSettings _levelSettings;

        public LevelRepository(LevelSettings levelSettings)
        {
            _levelSettings = levelSettings;
        }
        
        public Level Get(long id)
        {
            var levels = _levelSettings.Levels;
            if (levels.Count < id)
            {
                throw new InvalidOperationException("Level index is outside configured values");
            }

            var level = levels[(int) id - 1];
            return new Level(id, level.AsteroidCount, level.AsteroidChildCount);
        }
    }
}