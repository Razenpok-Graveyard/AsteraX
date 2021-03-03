using Common.Domain;

namespace AsteraX.Domain.Game
{
    public class GameSession : AggregateRoot
    {
        public GameSession(int initialJumps)
        {
            Contract.Requires(initialJumps >= 0, "initialJumps >= 0");

            Jumps = initialJumps;
        }

        public int Jumps { get; private set; }

        public int Score { get; set; }

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

        private void KillPlayer()
        {
            AddDomainEvent(new PlayerShipDestroyedEvent());

            if (Jumps == 0)
            {
                AddDomainEvent(new GameOverEvent());
            }
            else
            {
                Jumps--;
            }
        }
    }
}