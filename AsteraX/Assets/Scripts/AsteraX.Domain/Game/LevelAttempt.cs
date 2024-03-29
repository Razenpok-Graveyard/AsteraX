using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AsteraX.Domain.Game
{
    internal class LevelAttempt
    {
        private static long _lastAsteroidId;

        private readonly Collection<Asteroid> _asteroids;

        public LevelAttempt(Level level)
        {
            Contract.Requires(level != null, "level != null");

            _asteroids = GenerateAsteroids(level);
        }

        public IReadOnlyList<Asteroid> Asteroids => _asteroids;

        public bool IsAsteroidAlive(long asteroidId) => Asteroids.Any(a => a.Id == asteroidId);

        public Asteroid GetAsteroid(long asteroidId)
        {
            Contract.Requires(IsAsteroidAlive(asteroidId), "IsAsteroidAlive(asteroidId)");

            return _asteroids.First(a => a.Id == asteroidId);
        }

        public void Destroy(long asteroidId)
        {
            Contract.Requires(IsAsteroidAlive(asteroidId), "IsAsteroidAlive(asteroid)");

            var asteroid = GetAsteroid(asteroidId);
            _asteroids.Remove(asteroid);
            foreach (var child in asteroid.Children)
            {
                _asteroids.Add(child);
            }
        }

        private static Collection<Asteroid> GenerateAsteroids(Level level)
        {
            var asteroids = new Collection<Asteroid>();

            for (var i = 0; i < level.AsteroidCount; i++)
            {
                var asteroid = new Asteroid(_lastAsteroidId++, level.InitialAsteroidSize);
                PopulateAsteroidChildren(asteroid, level.AsteroidChildCount);
                asteroids.Add(asteroid);
            }

            return asteroids;
        }

        private static void PopulateAsteroidChildren(Asteroid asteroid, int childCount)
        {
            if (asteroid.Size <= 1)
            {
                return;
            }

            for (var i = 0; i < childCount; i++)
            {
                var child = new Asteroid(GetNextAsteroidId(), asteroid.Size - 1);
                PopulateAsteroidChildren(child, childCount);
                asteroid.Children.Add(child);
            }
        }

        private static long GetNextAsteroidId() => _lastAsteroidId++;
    }
}