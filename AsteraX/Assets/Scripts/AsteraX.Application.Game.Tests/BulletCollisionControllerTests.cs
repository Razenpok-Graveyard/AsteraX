using System.Collections;
using System.Linq;
using AsteraX.Application.Game.Requests;
using AsteraX.Application.UI.Requests;
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
                var mediator = new OutputMediatorSpy();
                var repository = new GameSessionRepository();
                var level = new Level(1, 3, 0);
                var levelRepository = new StubLevelRepository(level);
                var gameSession = repository.Get();
                gameSession.StartLevel(level);
                var asteroidId = gameSession.GetAsteroids().First().Id;
                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    repository,
                    mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                mediator
                    .HandleRequest<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_bullet_with_last_asteroid()
            => UniTask.ToCoroutine(async () =>
            {
                var mediator = new OutputMediatorSpy();
                var repository = new GameSessionRepository();
                var firstLevel = new Level(1, 1, 0);
                var secondLevel = new Level(2, 2, 3);
                var levelRepository = new StubLevelRepository(firstLevel, secondLevel);
                var gameSession = repository.Get();
                gameSession.StartLevel(firstLevel);
                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    repository,
                    mediator
                );
                var asteroidId = gameSession.GetAsteroids().First().Id;
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                mediator
                    .HandleRequest<DisablePlayerInput>()
                    .HandleRequest<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .HandleAsyncRequest<ShowLoadingScreen>(task =>
                    {
                        task.Id.Should().Be(2);
                        task.Asteroids.Should().Be(2);
                        task.Children.Should().Be(3);
                    })
                    .HandleRequest<SpawnAsteroids>(task =>
                    {
                        task.ShouldBeConsistentWithLevel(secondLevel);
                    })
                    .HandleAsyncRequest<HideLoadingScreen>()
                    .HandleRequest<EnablePlayerInput>()
                    .HandleRequest<UnpauseGame>()
                    .Complete();
            });
    }
}