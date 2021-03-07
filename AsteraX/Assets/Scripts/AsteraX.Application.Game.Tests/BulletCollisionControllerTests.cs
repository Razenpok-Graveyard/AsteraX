using System.Linq;
using AsteraX.Infrastructure.Data;
using Common.Application;
using FluentAssertions;
using NUnit.Framework;
using Razensoft.Functional;
using static AsteraX.Application.Game.Bullets.BulletCollisionController;

namespace AsteraX.Application.Game.Tests
{
    public class BulletCollisionControllerTests
    {
        [Test]
        public void Colliding_asteroid_with_bullet()
        {
            var repository = new GameSessionRepository();
            var gameSession = repository.Get();
            var level = new Domain.Game.Level(1, 1, 0);
            gameSession.StartLevel(level);
            var asteroidId = gameSession.GetAsteroids().First().Id;

            var taskPublisher = new FakeApplicationTaskPublisher();

            IRequestHandler<Command, Result> sut = new CommandHandler(repository, taskPublisher);
            var command = new Command
            {
                AsteroidId = asteroidId
            };
            var result = sut.Handle(command);

            result.IsSuccess.Should().BeTrue();
            taskPublisher
                .ShouldContainSingle<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId));
        }
    }
}