using System.Collections.Generic;
using Razensoft.Domain;

namespace AsteraX.Domain.Game
{
    public class GameSession : AggregateRoot
    {
        private readonly int _initialJumps;
        private LevelAttempt _levelAttempt;

        public GameSession(int initialJumps)
        {
            Contract.Requires(initialJumps >= 0, "initialJumps >= 0");

            _initialJumps = initialJumps;
            Jumps = initialJumps;
            IsPlayerAlive = true;
        }

        public int Jumps { get; private set; }

        public bool IsPlayerAlive { get; private set; }

        public Level Level { get; private set; }

        public bool CanShoot => IsPlayerAlive;

        public bool IsOver { get; private set; }

        public int Score { get; private set; }

        public bool IsPlayingLevel => _levelAttempt != null && Level != null;

        public bool IsLevelCompleted => GetAsteroids().Count == 0;

        public IReadOnlyList<Asteroid> GetAsteroids()
        {
            Contract.Requires(IsPlayingLevel, "IsPlayingLevel");
            return _levelAttempt.Asteroids;
        }

        public void StartLevel(Level level)
        {
            Level = level;
            _levelAttempt = new LevelAttempt(level);
        }

        public bool IsAsteroidAlive(long asteroidId)
        {
            return _levelAttempt.IsAsteroidAlive(asteroidId);
        }

        public void CollideAsteroidWithBullet(long asteroidId)
        {
            Contract.Requires(IsAsteroidAlive(asteroidId), "IsAsteroidAlive(asteroidId)");
            var asteroid = _levelAttempt.GetAsteroid(asteroidId);
            _levelAttempt.Destroy(asteroidId);
            Score += asteroid.Score;
        }

        public void CollideAsteroidWithPlayerShip(long asteroidId)
        {
            Contract.Requires(IsPlayerAlive, "IsPlayerAlive");
            Contract.Requires(IsAsteroidAlive(asteroidId), "IsAsteroidAlive(asteroidId)");
            _levelAttempt.Destroy(asteroidId);
            IsPlayerAlive = false;

            if (Jumps == 0)
            {
                IsOver = true;
            }
            else
            {
                Jumps--;
            }
        }

        public void RespawnPlayer()
        {
            Contract.Requires(!IsPlayerAlive, "!IsPlayerAlive");
            IsPlayerAlive = true;
        }

        public void Restart()
        {
            Contract.Requires(IsOver, "IsOver");
            IsOver = false;
            IsPlayerAlive = true;
            Jumps = _initialJumps;
            Score = 0;
            Level = null;
            _levelAttempt = null;
        }
    }
}