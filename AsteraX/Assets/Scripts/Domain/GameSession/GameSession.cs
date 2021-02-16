namespace AsteraX.Domain.GameSession
{
    public class GameSession : AggregateRoot
    {
        public GameSession(int initialJumps)
        {
            Jumps = initialJumps;
        }

        public int Jumps { get; private set; }

        public void KillPlayer()
        {
            AddDomainEvent(new PlayerDeathEvent());

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