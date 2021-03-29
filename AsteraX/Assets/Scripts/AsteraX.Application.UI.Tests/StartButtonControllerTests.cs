using System.Collections;
using AsteraX.Application.Game.Requests;
using AsteraX.Application.Tests;
using AsteraX.Application.UI.Requests;
using AsteraX.Domain.Game;
using AsteraX.Infrastructure;
using Razensoft.Mediator;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using UnityEngine.TestTools;
using static AsteraX.Application.UI.MainMenu.StartButtonController;

namespace AsteraX.Application.UI.Tests
{
    public class StartButtonControllerTests : IntegrationTests
    {
        [UnityTest]
        public IEnumerator Pressing_start_button()
            => UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var gameSessionRepository = new GameSessionRepository(SaveFile);
                var level = new Level(1, 2, 3);
                var levelRepository = new LevelRepositoryStub(level);

                IAsyncInputRequestHandler<Command> sut = new CommandHandler(
                    levelRepository,
                    gameSessionRepository,
                    Mediator
                );
                var command = new Command();

                // Act
                await sut.Handle(command);

                // Assert
                HandleRequest<HideMainMenuScreen>();
                HandleAsyncRequest<ShowLoadingScreen>(task =>
                {
                    task.Id.Should().Be(1);
                    task.Asteroids.Should().Be(2);
                    task.Children.Should().Be(3);
                });
                HandleRequest<SpawnAsteroids>(task =>
                {
                    task.ShouldBeConsistentWithLevel(level);
                });
                HandleAsyncRequest<HideLoadingScreen>();
                HandleRequest<UnpauseGame>();
                HandleRequest<EnablePlayerInput>();
                Complete();
            });
    }
}