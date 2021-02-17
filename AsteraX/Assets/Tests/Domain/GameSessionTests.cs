using AsteraX.Domain.GameSession;
using FluentAssertions;
using NUnit.Framework;

namespace AsteraX.Tests.Domain
{
    public class GameSessionTests
    {
        [Test]
        public void Killing_player_should_decrease_jumps()
        {
            const int initialJumps = 3;
            var session = new GameSession(initialJumps);
            
            session.KillPlayer();

            const int expectedJumps = 2;
            session.Jumps.Should().Be(expectedJumps);
        }

        [Test]
        public void Killing_player_should_trigger_player_death()
        {
            const int initialJumps = 3;
            var session = new GameSession(initialJumps);
            
            session.KillPlayer();

            session.DomainEvents.Should().ContainSingle(e => e is PlayerDeathEvent);
        }

        [Test]
        public void Killing_player_should_cause_game_over_when_there_are_no_jumps_remaining()
        {
            const int initialJumps = 0;
            var session = new GameSession(initialJumps);

            session.KillPlayer();

            session.DomainEvents.Should().ContainSingle(e => e is GameOverEvent);
        }

        [Test]
        public void Killing_player_should_not_cause_game_over_when_there_are_jumps_remaining()
        {
            const int initialJumps = 3;
            var session = new GameSession(initialJumps);
            
            session.KillPlayer();

            session.DomainEvents.Should().NotContain(e => e is GameOverEvent);
        }
    }
}