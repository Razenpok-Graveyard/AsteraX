using System.Collections;
using AsteraX.Application.Game.Tasks;
using AsteraX.Application.UI.Tasks;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure.Data;
using Common.Application;
using Common.Application.Tests;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using UnityEngine.TestTools;
using static AsteraX.Application.UI.MainMenu.StartButtonController;

namespace AsteraX.Application.UI.Tests
{
    public class StartButtonControllerTests
    {
        [UnityTest]
        public IEnumerator Pressing_start_button()
            => UniTask.ToCoroutine(async () =>
            {
                var taskPublisher = new ApplicationTaskPublisherSpy();
                var gameSessionRepository = new GameSessionRepository();
                var level = new Level(1, 2, 3);
                var levelRepository = new StubLevelRepository(level);
                IAsyncRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    taskPublisher
                );
                var command = new Command();

                await sut.Handle(command);

                taskPublisher
                    .Consume<HideMainMenuScreen>()
                    .ConsumeAsync<ShowLoadingScreen>(task =>
                    {
                        task.Id.Should().Be(1);
                        task.Asteroids.Should().Be(2);
                        task.Children.Should().Be(3);
                    })
                    .Consume<SpawnAsteroids>(task => task.ShouldBeConsistentWithLevel(level))
                    .ConsumeAsync<HideLoadingScreen>()
                    .Consume<ShowPauseButton>()
                    .Consume<UnpauseGame>()
                    .Consume<EnablePlayerInput>()
                    .Complete();
            });
    }
}