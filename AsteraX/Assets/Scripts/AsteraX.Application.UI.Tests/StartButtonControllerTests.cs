using System.Collections;
using AsteraX.Application.Game.Requests;
using AsteraX.Application.UI.Requests;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Razensoft.Mediator;
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
                var mediator = new OutputMediatorSpy();
                var gameSessionRepository = new GameSessionRepository();
                var level = new Level(1, 2, 3);
                var levelRepository = new StubLevelRepository(level);
                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    mediator
                );
                var command = new Command();

                await sut.Handle(command);

                mediator
                    .HandleRequest<HideMainMenuScreen>()
                    .HandleAsyncRequest<ShowLoadingScreen>(task =>
                    {
                        task.Id.Should().Be(1);
                        task.Asteroids.Should().Be(2);
                        task.Children.Should().Be(3);
                    })
                    .HandleRequest<SpawnAsteroids>(task =>
                    {
                        task.ShouldBeConsistentWithLevel(level);
                    })
                    .HandleAsyncRequest<HideLoadingScreen>()
                    //.HandleRequest<ShowPauseButton>()
                    .HandleRequest<UnpauseGame>()
                    .HandleRequest<EnablePlayerInput>()
                    .Complete();
            });
    }
}