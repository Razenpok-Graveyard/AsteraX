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
        public IEnumerator Colliding_asteroid_with_bullet() => UniTask.ToCoroutine(
            async () =>
            {
                const int asteroidCount = 3;
                var levelRepository = new LevelRepository();
                var repository = new GameSessionRepository();
                var gameSession = repository.Get();
                var level = new Level(1, 1, 0);
                gameSession.StartLevel(level);
                var asteroidId = gameSession.GetAsteroids().First().Id;

                var taskPublisher = new FakeApplicationTaskPublisher();

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
                    .ConsumeAsync<ShowLoadingScreen>(task =>
                    {
                        task.Id.Should().Be(1);
                        task.Asteroids.Should().Be(3);
                        task.Children.Should().Be(3);
                    })
                    .Consume<SpawnAsteroids>(task => task.Asteroids.Count.Should().Be(asteroidCount))
                    .ConsumeAsync<HideLoadingScreen>()
                    .Consume<EnablePlayerInput>()
                    .Consume<UnpauseGame>()
                    .Complete();
            });
    }
}