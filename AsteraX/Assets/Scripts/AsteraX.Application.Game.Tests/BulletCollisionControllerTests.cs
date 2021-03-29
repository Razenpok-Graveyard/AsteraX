using System.Collections;
using System.Linq;
using AsteraX.Application.Game.Notifications;
using AsteraX.Application.Game.Requests;
using AsteraX.Application.Tests;
using AsteraX.Application.UI.Requests;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Razensoft.Mediator;
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
                var levelRepository = new LevelRepositoryStub(level);
                var gameSession = repository.Get();
                gameSession.StartLevel(level);
                var asteroid = gameSession.GetAsteroids().First();
                var asteroidId = asteroid.Id;
                var asteroidScore = asteroid.Score;
                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    repository,
                    mediator,
                    null
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                mediator
                    .HandleRequest<DestroyAsteroid>(request => request.Id.Should().Be(asteroidId))
                    .HandleNotification<AsteroidShot>(notification =>
                    {
                        notification.IsLuckyShot.Should().BeFalse();
                    })
                    .HandleNotification<HighScoreUpdated>(notification =>
                    {
                        notification.Score.Should().Be(asteroidScore);
                    })
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_lucky_shot_with_asteroid()
            => UniTask.ToCoroutine(async () =>
            {
                var mediator = new OutputMediatorSpy();
                var repository = new GameSessionRepository();
                var level = new Level(1, 3, 0);
                var levelRepository = new LevelRepositoryStub(level);
                var gameSession = repository.Get();
                gameSession.StartLevel(level);
                var asteroid = gameSession.GetAsteroids().First();
                var asteroidId = asteroid.Id;
                var asteroidScore = asteroid.Score;
                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    repository,
                    mediator,
                    null
                );
                var command = new Command
                {
                    AsteroidId = asteroidId,
                    IsLuckyShot = true
                };

                await sut.Handle(command);

                mediator
                    .HandleRequest<DestroyAsteroid>(request => request.Id.Should().Be(asteroidId))
                    .HandleNotification<AsteroidShot>(notification =>
                    {
                        notification.IsLuckyShot.Should().BeTrue();
                    })
                    .HandleNotification<HighScoreUpdated>(notification =>
                    {
                        notification.Score.Should().Be(asteroidScore);
                    })
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
                var levelRepository = new LevelRepositoryStub(firstLevel, secondLevel);
                var gameSession = repository.Get();
                gameSession.StartLevel(firstLevel);
                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    repository,
                    mediator,
                    null
                );
                var asteroid = gameSession.GetAsteroids().First();
                var asteroidId = asteroid.Id;
                var asteroidScore = asteroid.Score;
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                mediator
                    .HandleRequest<DisablePlayerInput>()
                    .HandleRequest<DestroyAsteroid>(request => request.Id.Should().Be(asteroidId))
                    .HandleNotification<AsteroidShot>()
                    .HandleNotification<HighScoreUpdated>(notification =>
                    {
                        notification.Score.Should().Be(asteroidScore);
                    })
                    .HandleNotification<LevelReached>(notification =>
                    {
                        notification.Id.Should().Be(2);
                    })
                    .HandleAsyncRequest<ShowLoadingScreen>(request =>
                    {
                        request.Id.Should().Be(2);
                        request.Asteroids.Should().Be(2);
                        request.Children.Should().Be(3);
                    })
                    .HandleRequest<SpawnAsteroids>(request =>
                    {
                        request.ShouldBeConsistentWithLevel(secondLevel);
                    })
                    .HandleAsyncRequest<HideLoadingScreen>()
                    .HandleRequest<EnablePlayerInput>()
                    .HandleRequest<UnpauseGame>()
                    .Complete();
            });
    }
}