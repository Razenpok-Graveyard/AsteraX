using System.Linq;
using AsteraX.Application.Game;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure.Data;
using Common.Application;
using Common.Functional;
using FluentAssertions;
using NUnit.Framework;
using static AsteraX.Application.Game.Player.PlayerShipCollisionController;

namespace AsteraX.Application.Tests
{
    public class PlayerShipCollisionControllerTests
    {
        [Test]
        public void Colliding_asteroid_with_player_when_there_are_jumps_remaining()
        {
            var observableModelRepository = new GameSessionObservableModelRepository();
            var repository = new GameSessionRepository(observableModelRepository);
            var gameSession = repository.GetCurrentSession();
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
                .ShouldContainSingle<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                .ShouldContainSingle<DestroyPlayerShip>()
                .ShouldContainSingle<RespawnPlayerShip>();
        }

        [Test]
        public void Colliding_asteroid_with_player_when_there_are_no_jumps_remaining()
        {
            var observableModelRepository = new GameSessionObservableModelRepository();
            var gameSessionSettings = new GameSessionSettings
            {
                InitialJumps = 0
            };
            var repository = new GameSessionRepository(observableModelRepository, gameSessionSettings);
            var gameSession = repository.GetCurrentSession();
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
                .ShouldContainSingle<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                .ShouldContainSingle<DestroyPlayerShip>();
        }
    }
}