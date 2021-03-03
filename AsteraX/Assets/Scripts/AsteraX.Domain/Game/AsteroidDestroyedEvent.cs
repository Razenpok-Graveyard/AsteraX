using Common.Domain;

namespace AsteraX.Domain.Game
{
    public class AsteroidDestroyedEvent : IDomainEvent
    {
        public long Id { get; set; }
    }
}