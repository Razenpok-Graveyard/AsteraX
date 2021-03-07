using System.Collections.Generic;
using Common.Domain;

namespace AsteraX.Domain.Game
{
    public class GameSession : AggregateRoot
    {
        private LevelAttempt _levelAttempt;

        public GameSession(int initialJumps)
        {
            Contract.Requires(initialJumps >= 0, "initialJumps >= 0");

            Jumps = initialJumps;
            IsPlayerAlive = true;
        }

        public int Jumps { get; private set; }

        public bool IsPlayerAlive { get; private set; }

        public bool CanShoot => IsPlayerAlive;

        public bool IsOver { get; private set; }

        public int Score { get; private set; }

        public bool IsPlayingLevel => _levelAttempt != null;

        public IReadOnlyCollection<Asteroid> GetAsteroids()
        {
            Contract.Requires(IsPlayingLevel, "IsPlayingLevel");
            return _levelAttempt.Asteroids;
        }

        public void StartLevel(Level level)
        {
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
            AddDomainEvent(new PlayerShipDestroyedEvent());

            if (Jumps == 0)
            {
                AddDomainEvent(new GameOverEvent());
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
    }
}