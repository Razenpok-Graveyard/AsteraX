using System.Collections;
using System.Linq;
using AsteraX.Application.Game.Notifications;
using AsteraX.Application.Game.Requests;
using AsteraX.Application.UI.Requests;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using Razensoft.Mediator;
using UnityEngine.TestTools;
using static AsteraX.Application.Game.Player.PlayerShipCollisionController;

namespace AsteraX.Application.Tests.Game
{
    public class PlayerShipCollisionControllerTests : IntegrationTests
    {
        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player()
            => UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var gameSessionRepository = new GameSessionRepository(SaveFile);
                var level = new Level(1, 3, 0);
                var levelRepository = new LevelRepositoryStub(level);
                var gameSession = gameSessionRepository.Get();
                gameSession.StartLevel(level);
                var asteroidId = gameSession.GetAsteroids().First().Id;

                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    Mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                // Act
                await sut.Handle(command);

                // Assert
                VerifyRequest<DisablePlayerInput>();
                VerifyRequest<DestroyPlayerShip>();
                VerifyRequest<DestroyAsteroid>(request => request.Id.Should().Be(asteroidId));
                VerifyAsyncRequest<RespawnPlayerShipWithVisuals>();
                VerifyRequest<EnablePlayerInput>();
                CompleteMediatorVerification();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_all_asteroids_are_destroyed()
            => UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var gameSessionRepository = new GameSessionRepository(SaveFile);
                var gameSession = gameSessionRepository.Get();
                var firstLevel = new Level(1, 1, 0);
                var secondLevel = new Level(2, 2, 3);
                var levelRepository = new LevelRepositoryStub(firstLevel, secondLevel);
                gameSession.StartLevel(firstLevel);
                var asteroidId = gameSession.GetAsteroids().First().Id;

                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    Mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                // Act
                await sut.Handle(command);

                // Assert
                VerifyRequest<DisablePlayerInput>();
                VerifyRequest<DestroyPlayerShip>();
                VerifyRequest<DestroyAsteroid>(request =>
                {
                    request.Id.Should().Be(asteroidId);
                });
                VerifyNotification<LevelReached>(notification =>
                {
                    notification.Id.Should().Be(2);
                });
                VerifyAsyncRequest<ShowLoadingScreen>(request =>
                {
                    request.Id.Should().Be(2);
                    request.Asteroids.Should().Be(2);
                    request.Children.Should().Be(3);
                });
                VerifyRequest<SpawnAsteroids>(request =>
                {
                    request.ShouldBeConsistentWithLevel(secondLevel);
                });
                VerifyRequest<RespawnPlayerShip>(request =>
                {
                    request.IntoInitialPosition.Should().BeFalse();
                });
                VerifyAsyncRequest<HideLoadingScreen>();
                VerifyRequest<UnpauseGame>();
                VerifyRequest<EnablePlayerInput>();
                CompleteMediatorVerification();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_there_are_no_jumps_remaining()
            => UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var gameSessionSettings = new GameSessionSettings
                {
                    InitialJumps = 0
                };
                var gameSessionRepository = new GameSessionRepository(SaveFile, gameSessionSettings);
                var level = new Level(1, 3, 0);
                var levelRepository = new LevelRepositoryStub(level);
                var gameSession = gameSessionRepository.Get();
                gameSession.StartLevel(level);
                var asteroids = gameSession.GetAsteroids();
                var killedAsteroidId = asteroids[0].Id;
                var killedAsteroidScore = asteroids[0].Score;
                var asteroidId = asteroids[1].Id;
                // Kill asteroid to test game over screen score
                gameSession.CollideAsteroidWithBullet(killedAsteroidId);
                gameSessionRepository.Save();

                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    Mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                // Act
                await sut.Handle(command);

                // Assert
                VerifyRequest<DisablePlayerInput>();
                VerifyRequest<DestroyPlayerShip>();
                VerifyRequest<DestroyAsteroid>(request => request.Id.Should().Be(asteroidId));
                VerifyAsyncRequest<ShowGameOverScreen>(request =>
                {
                    request.Level.Should().Be(1);
                    request.Score.Should().Be(killedAsteroidScore);
                });
                VerifyRequest<ClearAsteroids>();
                VerifyRequest<RespawnPlayerShip>(request =>
                {
                    request.IntoInitialPosition.Should().BeTrue();
                });
                VerifyAsyncRequest<HideGameOverScreen>();
                VerifyRequest<ShowMainMenuScreen>();
                CompleteMediatorVerification();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_there_are_no_jumps_remaining_and_all_asteroids_are_destroyed()
            => UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var gameSessionSettings = new GameSessionSettings
                {
                    InitialJumps = 0
                };
                var gameSessionRepository = new GameSessionRepository(SaveFile, gameSessionSettings);
                var level = new Level(1, 2, 0);
                var levelRepository = new LevelRepositoryStub(level);
                var gameSession = gameSessionRepository.Get();
                gameSession.StartLevel(level);
                var asteroids = gameSession.GetAsteroids();
                var killedAsteroidId = asteroids[0].Id;
                var killedAsteroidScore = asteroids[0].Score;
                var asteroidId = asteroids[1].Id;
                // Kill asteroid to test game over screen score
                gameSession.CollideAsteroidWithBullet(killedAsteroidId);
                gameSessionRepository.Save();

                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    Mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                // Act
                await sut.Handle(command);

                // Assert
                VerifyRequest<DisablePlayerInput>();
                VerifyRequest<DestroyPlayerShip>();
                VerifyRequest<DestroyAsteroid>(request => request.Id.Should().Be(asteroidId));
                VerifyAsyncRequest<ShowGameOverScreen>(request =>
                {
                    request.Level.Should().Be(1);
                    request.Score.Should().Be(killedAsteroidScore);
                });
                VerifyRequest<ClearAsteroids>();
                VerifyRequest<RespawnPlayerShip>(request =>
                {
                    request.IntoInitialPosition.Should().BeTrue();
                });
                VerifyAsyncRequest<HideGameOverScreen>();
                VerifyRequest<ShowMainMenuScreen>();
                CompleteMediatorVerification();
            });
    }
}