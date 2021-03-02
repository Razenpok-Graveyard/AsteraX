using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace AsteraX.Domain.Game
{
    public class Level : Entity
    {
        public Level(long id, int asteroidCount, int asteroidChildCount) : base(id)
        {
            Contract.Requires(id > 0, "id > 0");
            Contract.Requires(asteroidCount > 0, "asteroidCount > 0");
            Contract.Requires(asteroidChildCount >= 0, "asteroidChildCount >= 0");

            AsteroidCount = asteroidCount;
            AsteroidChildCount = asteroidChildCount;
        }
        
        public int AsteroidCount { get; }

        public int AsteroidChildCount { get; }

        public int InitialAsteroidSize => 3;
    }
}