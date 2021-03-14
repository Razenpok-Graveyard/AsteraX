using System;
using System.Collections;
using System.Linq;
using AsteraX.Application.Tasks.Game;
using AsteraX.Application.Tasks.UI;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure.Data;
using Common.Application;
using Common.Application.Tests;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using UnityEngine.TestTools;
using static AsteraX.Application.Game.Player.PlayerShipCollisionController;

namespace AsteraX.Application.Game.Tests
{
    public class PlayerShipCollisionControllerTests
    {
        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player()
            => UniTask.ToCoroutine(async () =>
            {
                var repository = new GameSessionRepository();
                var level = new Level(1, 3, 0);
                var levelRepository = new StubLevelRepository(level);
                var gameSession = repository.Get();
                gameSession.StartLevel(level);
                var asteroidId = gameSession.GetAsteroids().First().Id;

                var taskPublisher = new ApplicationTaskPublisherSpy();

                IAsyncRequestHandler<Command> sut = new CommandHandler(levelRepository, repository, taskPublisher);
                var command = new Command
                {
                    AsteroidId = asteroidId
                };
                await sut.Handle(command);

                taskPublisher
                    .Consume<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .Consume<DestroyPlayerShip>()
                    .ConsumeAsync<RespawnPlayerShip>(task =>
                    {
                        task.SpawnEffects.Should().BeTrue();
                    })
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_all_asteroids_are_destroyed()
            => UniTask.ToCoroutine(async () =>
            {
                var repository = new GameSessionRepository();
                var gameSession = repository.Get();
                var firstLevel = new Level(1, 1, 0);
                var secondLevel = new Level(1, 3, 3);
                var levelRepository = new StubLevelRepository(firstLevel, secondLevel);
                gameSession.StartLevel(firstLevel);
                var asteroidId = gameSession.GetAsteroids().First().Id;

                var taskPublisher = new ApplicationTaskPublisherSpy();

                IAsyncRequestHandler<Command> sut = new CommandHandler(levelRepository, repository, taskPublisher);
                var command = new Command
                {
                    AsteroidId = asteroidId
                };
                await sut.Handle(command);

                taskPublisher
                    .Consume<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .Consume<DestroyPlayerShip>()
                    .ConsumeAsync<ShowLoadingScreen>(task =>
                    {
                        task.Id.Should().Be(1);
                        task.Asteroids.Should().Be(1);
                        task.Children.Should().Be(0);
                    })
                    .Consume<SpawnAsteroids>(task => task.ShouldBeConsistentWithLevel(firstLevel))
                    .ConsumeAsync<RespawnPlayerShip>(task =>
                    {
                        task.Delay.Should().Be(TimeSpan.Zero);
                        task.SpawnEffects.Should().BeFalse();
                    })
                    .ConsumeAsync<HideLoadingScreen>()
                    .Consume<UnpauseGame>()
                    .Consume<EnablePlayerInput>()
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_there_are_no_jumps_remaining()
            => UniTask.ToCoroutine(async () =>
            {
                var taskPublisher = new ApplicationTaskPublisherSpy();
                var gameSessionSettings = new GameSessionSettings
                {
                    InitialJumps = 0
                };
                var repository = new GameSessionRepository(gameSessionSettings);
                var level = new Level(1, 3, 0);
                var levelRepository = new StubLevelRepository(level);
                var gameSession = repository.Get();
                gameSession.StartLevel(level);
                var asteroids = gameSession.GetAsteroids();
                var killedAsteroidId = asteroids[0].Id;
                var killedAsteroidScore = asteroids[0].Score;
                var asteroidId = asteroids[1].Id;
                // Kill asteroid to test game over screen score
                gameSession.CollideAsteroidWithBullet(killedAsteroidId);
                repository.Save();

                IAsyncRequestHandler<Command> sut = new CommandHandler(levelRepository, repository, taskPublisher);
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                taskPublisher
                    .Consume<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .Consume<DestroyPlayerShip>()
                    .ConsumeAsync<ShowGameOverScreen>(task =>
                    {
                        task.Level.Should().Be(1);
                        task.Score.Should().Be(killedAsteroidScore);
                    })
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_there_are_no_jumps_remaining_and_all_asteroids_are_destroyed()
            => UniTask.ToCoroutine(async () =>
            {
                var taskPublisher = new ApplicationTaskPublisherSpy();
                var gameSessionSettings = new GameSessionSettings
                {
                    InitialJumps = 0
                };
                var repository = new GameSessionRepository(gameSessionSettings);
                var level = new Level(1, 2, 0);
                var levelRepository = new StubLevelRepository(level);
                var gameSession = repository.Get();
                gameSession.StartLevel(level);
                var asteroids = gameSession.GetAsteroids();
                var killedAsteroidId = asteroids[0].Id;
                var killedAsteroidScore = asteroids[0].Score;
                var asteroidId = asteroids[1].Id;
                // Kill asteroid to test game over screen score
                gameSession.CollideAsteroidWithBullet(killedAsteroidId);
                repository.Save();
                IAsyncRequestHandler<Command> sut = new CommandHandler(levelRepository, repository, taskPublisher);
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                taskPublisher
                    .Consume<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .Consume<DestroyPlayerShip>()
                    .ConsumeAsync<ShowGameOverScreen>(task =>
                    {
                        task.Level.Should().Be(1);
                        task.Score.Should().Be(killedAsteroidScore);
                    })
                    .Complete();
            });
    }
}