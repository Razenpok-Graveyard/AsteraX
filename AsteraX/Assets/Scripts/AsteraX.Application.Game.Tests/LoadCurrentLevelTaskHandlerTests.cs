using System.Collections;
using AsteraX.Application.UI;
using AsteraX.Infrastructure.Data;
using Common.Application;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using UnityEngine.TestTools;
using static AsteraX.Application.Game.Levels.LoadCurrentLevelTaskHandler;

namespace AsteraX.Application.Game.Tests
{
    public class LoadCurrentLevelTaskHandlerTests
    {
        [UnityTest]
        public IEnumerator Starting_new_game() => UniTask.ToCoroutine(
            async () =>
            {
                const int asteroidCount = 3;
                var gameSessionRepository = new GameSessionRepository();
                var taskPublisher = new FakeApplicationTaskPublisher();
                IAsyncRequestHandler<Command> sut = new CommandHandler(
                    gameSessionRepository,
                    taskPublisher
                );
                var command = new Command();

                await sut.Handle(command);

                taskPublisher
                    .ShouldContainSingle<ShowLoadingScreen>(task =>
                    {
                        task.Id.Should().Be(1);
                        task.Asteroids.Should().Be(3);
                        task.Children.Should().Be(3);
                    })
                    .ShouldContainSingle<SpawnAsteroids>(task =>
                    {
                        task.Asteroids.Count.Should().Be(asteroidCount);
                    })
                    .ShouldContainSingle<HideLoadingScreen>();
            });
    }
}