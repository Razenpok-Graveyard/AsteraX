using System.Linq;
using AsteraX.Application.Game;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure.Data;
using Common.Application;
using Common.Functional;
using FluentAssertions;
using NUnit.Framework;
using static AsteraX.Application.Game.Bullets.BulletCollisionController;

namespace AsteraX.Application.Tests
{
    public class BulletCollisionControllerTests
    {
        [Test]
        public void Colliding_asteroid_with_bullet()
        {
            var repository = new GameSessionRepository();
            var gameSession = repository.Get();
            var level = new Level(1, 1, 0);
            gameSession.StartLevel(level);
            var asteroidId = gameSession.LevelAttempt.Asteroids.First().Id;

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