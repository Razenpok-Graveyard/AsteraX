using Common.Domain;

namespace AsteraX.Domain.Game
{
    public class GameSession : AggregateRoot
    {
        public GameSession(int initialJumps)
        {
            Contract.Requires(initialJumps >= 0, "initialJumps >= 0");

            Jumps = initialJumps;
            IsPlayerAlive = true;
        }

        public int Jumps { get; private set; }

        public bool IsPlayerAlive { get; private set; }

        public bool IsOver { get; private set; }

        public int Score { get; private set; }

        public LevelAttempt LevelAttempt { get; private set; }

        public void StartLevel(Level level)
        {
            LevelAttempt = new LevelAttempt(level);
        }

        public void CollideAsteroidWithBullet(Asteroid asteroid)
        {
            LevelAttempt.Destroy(asteroid);
            Score += asteroid.Score;
        }

        public void CollideAsteroidWithPlayerShip(Asteroid asteroid)
        {
            LevelAttempt.Destroy(asteroid);
            KillPlayer();
        }

        public void RespawnPlayer()
        {
            IsPlayerAlive = true;
        }

        private void KillPlayer()
        {
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
    }
}