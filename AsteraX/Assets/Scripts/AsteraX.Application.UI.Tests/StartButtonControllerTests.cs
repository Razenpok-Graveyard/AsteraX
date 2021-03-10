using System.Collections;
using AsteraX.Application.Game;
using AsteraX.Application.Game.Tests;
using AsteraX.Application.Tasks.Game;
using AsteraX.Application.Tasks.UI;
using AsteraX.Infrastructure.Data;
using Common.Application;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using UnityEngine.TestTools;
using static AsteraX.Application.UI.MainMenu.StartButtonController;

namespace AsteraX.Application.UI.Tests
{
    public class StartButtonControllerTests
    {
        [UnityTest]
        public IEnumerator Pressing_start_button() => UniTask.ToCoroutine(
            async () =>
            {
                const int asteroidCount = 3;
                var levelRepository = new LevelRepository();
                var gameSessionRepository = new GameSessionRepository();
                var taskPublisher = new FakeApplicationTaskPublisher();
                IAsyncRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    taskPublisher
                );
                var command = new Command();

                await sut.Handle(command);

                taskPublisher
                    .ShouldContainSingle<HideMainMenuScreen>()
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
                    .ShouldContainSingle<HideLoadingScreen>()
                    .ShouldContainSingle<ShowPauseButton>()
                    .ShouldContainSingle<UnpauseGame>()
                    .ShouldContainSingle<EnablePlayerInput>();
            });
    }
}