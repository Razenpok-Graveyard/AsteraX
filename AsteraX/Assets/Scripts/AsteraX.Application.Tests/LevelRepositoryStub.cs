using System.Collections.Generic;
using System.Linq;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;

namespace AsteraX.Application.Tests
{
    public class LevelRepositoryStub : ILevelRepository
    {
        private readonly List<Level> _levels;

        public LevelRepositoryStub(IEnumerable<Level> levels)
        {
            _levels = levels.ToList();
        }

        public LevelRepositoryStub(params Level[] levels)
        {
            _levels = levels.ToList();
        }

        public IReadOnlyList<Level> Levels => _levels;
        
        public Level Get(long id)
        {
            return _levels.First(l => l.Id == id);
        }
    }
}