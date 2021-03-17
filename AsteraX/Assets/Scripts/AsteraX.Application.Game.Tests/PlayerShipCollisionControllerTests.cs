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
using static AsteraX.Application.Game.Player.PlayerShipCollisionController;

namespace AsteraX.Application.Game.Tests
{
    public class PlayerShipCollisionControllerTests
    {
        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player()
            => UniTask.ToCoroutine(async () =>
            {
                var mediator = new OutputMediatorSpy();
                var gameSessionRepository = new GameSessionRepository();
                var level = new Level(1, 3, 0);
                var levelRepository = new StubLevelRepository(level);
                var gameSession = gameSessionRepository.Get();
                gameSession.StartLevel(level);
                var asteroidId = gameSession.GetAsteroids().First().Id;
                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                mediator
                    .HandleRequest<DisablePlayerInput>()
                    .HandleRequest<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .HandleRequest<DestroyPlayerShip>()
                    .HandleAsyncRequest<RespawnPlayerShipWithVisuals>()
                    .HandleRequest<EnablePlayerInput>()
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_all_asteroids_are_destroyed()
            => UniTask.ToCoroutine(async () =>
            {
                var mediator = new OutputMediatorSpy();
                var gameSessionRepository = new GameSessionRepository();
                var gameSession = gameSessionRepository.Get();
                var firstLevel = new Level(1, 1, 0);
                var secondLevel = new Level(2, 2, 3);
                var levelRepository = new StubLevelRepository(firstLevel, secondLevel);
                gameSession.StartLevel(firstLevel);
                var asteroidId = gameSession.GetAsteroids().First().Id;

                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };
                await sut.Handle(command);

                mediator
                    .HandleRequest<DisablePlayerInput>()
                    .HandleRequest<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .HandleRequest<DestroyPlayerShip>()
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
                    .HandleRequest<RespawnPlayerShip>(task =>
                    {
                        task.IntoInitialPosition.Should().BeFalse();
                    })
                    .HandleAsyncRequest<HideLoadingScreen>()
                    .HandleRequest<UnpauseGame>()
                    .HandleRequest<EnablePlayerInput>()
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_there_are_no_jumps_remaining()
            => UniTask.ToCoroutine(async () =>
            {
                var mediator = new OutputMediatorSpy();
                var gameSessionSettings = new GameSessionSettings
                {
                    InitialJumps = 0
                };
                var gameSessionRepository = new GameSessionRepository(gameSessionSettings);
                var level = new Level(1, 3, 0);
                var levelRepository = new StubLevelRepository(level);
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
                    mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                mediator
                    .HandleRequest<DisablePlayerInput>()
                    .HandleRequest<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .HandleRequest<DestroyPlayerShip>()
                    .HandleAsyncRequest<ShowGameOverScreen>(task =>
                    {
                        task.Level.Should().Be(1);
                        task.Score.Should().Be(killedAsteroidScore);
                    })
                    .HandleRequest<ClearAsteroids>()
                    .HandleRequest<RespawnPlayerShip>(task =>
                    {
                        task.IntoInitialPosition.Should().BeTrue();
                    })
                    .HandleAsyncRequest<HideGameOverScreen>()
                    .HandleRequest<ShowMainMenuScreen>()
                    .Complete();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_there_are_no_jumps_remaining_and_all_asteroids_are_destroyed()
            => UniTask.ToCoroutine(async () =>
            {
                var mediator = new OutputMediatorSpy();
                var gameSessionSettings = new GameSessionSettings
                {
                    InitialJumps = 0
                };
                var gameSessionRepository = new GameSessionRepository(gameSessionSettings);
                var level = new Level(1, 2, 0);
                var levelRepository = new StubLevelRepository(level);
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
                    mediator
                );
                var command = new Command
                {
                    AsteroidId = asteroidId
                };

                await sut.Handle(command);

                mediator
                    .HandleRequest<DisablePlayerInput>()
                    .HandleRequest<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .HandleRequest<DestroyPlayerShip>()
                    .HandleAsyncRequest<ShowGameOverScreen>(task =>
                    {
                        task.Level.Should().Be(1);
                        task.Score.Should().Be(killedAsteroidScore);
                    })
                    .HandleRequest<ClearAsteroids>()
                    .HandleRequest<RespawnPlayerShip>(task =>
                    {
                        task.IntoInitialPosition.Should().BeTrue();
                    })
                    .HandleAsyncRequest<HideGameOverScreen>()
                    .HandleRequest<ShowMainMenuScreen>()
                    .Complete();
            });
    }
}