using System.Collections;
using System.Linq;
using AsteraX.Application.Tasks.Game;
using AsteraX.Application.Tasks.UI;
using AsteraX.Infrastructure.Data;
using Common.Application;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using UnityEngine.TestTools;
using static AsteraX.Application.Game.Player.PlayerShipCollisionController;

namespace AsteraX.Application.Game.Tests
{
    public class PlayerShipCollisionControllerTests
    {
        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_there_are_jumps_remaining() => UniTask.ToCoroutine(
            async () =>
            {
                var repository = new GameSessionRepository();
                var gameSession = repository.Get();
                var level = new Domain.Game.Level(1, 1, 0);
                gameSession.StartLevel(level);
                var asteroidId = gameSession.GetAsteroids().First().Id;

                var taskPublisher = new FakeApplicationTaskPublisher();

                IAsyncRequestHandler<Command> sut = new CommandHandler(repository, taskPublisher);
                var command = new Command
                {
                    AsteroidId = asteroidId
                };
                await sut.Handle(command);

                taskPublisher
                    .Consume<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .Consume<DestroyPlayerShip>()
                    .Consume<RespawnPlayerShip>();
            });

        [UnityTest]
        public IEnumerator Colliding_asteroid_with_player_when_there_are_no_jumps_remaining() => UniTask.ToCoroutine(
            async () =>
            {
                var taskPublisher = new FakeApplicationTaskPublisher();
                var gameSessionSettings = new GameSessionSettings
                {
                    InitialJumps = 0
                };
                var repository = new GameSessionRepository(gameSessionSettings);
                var gameSession = repository.Get();

                var level = new Domain.Game.Level(1, 2, 0);
                gameSession.StartLevel(level);
                var asteroids = gameSession.GetAsteroids();
                var killedAsteroidId = asteroids[0];
                var asteroidId = asteroids[1].Id;
                gameSession.CollideAsteroidWithBullet(killedAsteroidId.Id);
                repository.Save();

                IAsyncRequestHandler<Command> sut = new CommandHandler(repository, taskPublisher);
                var command = new Command
                {
                    AsteroidId = asteroidId
                };
                
                await sut.Handle(command);

                taskPublisher
                    .Consume<DestroyAsteroid>(task => task.Id.Should().Be(asteroidId))
                    .Consume<DestroyPlayerShip>()
                    .Consume<ShowGameOverScreen>(task =>
                    {
                        task.Level.Should().Be(1);
                        task.Score.Should().Be(gameSession.Score);
                    });
            });
    }
}