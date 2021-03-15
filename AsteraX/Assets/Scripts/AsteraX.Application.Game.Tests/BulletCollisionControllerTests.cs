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
using static AsteraX.Application.Game.Bullets.BulletCollisionController;

namespace AsteraX.Application.Game.Tests
{
    public class BulletCollisionControllerTests
    {
        [UnityTest]
        public IEnumerator Colliding_bullet_with_asteroid()
            => UniTask.ToCoroutine(async () =>
            {
                var taskPublisher = new ApplicationTaskPublisherSpy();
                var repository = new GameSessionRepository();
                var level = new Level(1, 3, 0);
                var levelRepository = new StubLevelRepository(level);
                var gameSession = repository.Get();
                gameSession.StartLevel(level);
                var asteroidId = gameSession.GetAsteroids().First().Id;
                IAsyncRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    repository,
                    taskPublisher
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                taskPublisher
                    .Consume<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_bullet_with_last_asteroid()
            => UniTask.ToCoroutine(async () =>
            {
                var taskPublisher = new ApplicationTaskPublisherSpy();
                var repository = new GameSessionRepository();
                var firstLevel = new Level(1, 1, 0);
                var secondLevel = new Level(2, 2, 3);
                var levelRepository = new StubLevelRepository(firstLevel, secondLevel);
                var gameSession = repository.Get();
                gameSession.StartLevel(firstLevel);
                IAsyncRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    repository,
                    taskPublisher
                );
                var asteroidId = gameSession.GetAsteroids().First().Id;
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                taskPublisher
                    .Consume<DisablePlayerInput>()
                    .Consume<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .ConsumeAsync<ShowLoadingScreen>(task =>
                    {
                        task.Id.Should().Be(2);
                        task.Asteroids.Should().Be(2);
                        task.Children.Should().Be(3);
                    })
                    .Consume<SpawnAsteroids>(task => task.ShouldBeConsistentWithLevel(secondLevel))
                    .ConsumeAsync<HideLoadingScreen>()
                    .Consume<EnablePlayerInput>()
                    .Consume<UnpauseGame>()
                    .Complete();
            });
    }
}