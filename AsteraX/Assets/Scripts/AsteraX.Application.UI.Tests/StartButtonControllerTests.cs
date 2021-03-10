using System.Collections;
using AsteraX.Application.Game;
using AsteraX.Application.Game.Tests;
using AsteraX.Infrastructure.Data;
using Common.Application;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using static AsteraX.Application.UI.StartButtonController;

namespace AsteraX.Application.UI.Tests
{
    public class StartButtonControllerTests
    {
        [UnityTest]
        public IEnumerator Pressing_start_button() => UniTask.ToCoroutine(
            async () =>
            {
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

                sut.Handle(command);

                taskPublisher
                    .ShouldContainSingle<CloseMainMenu>()
                    .ShouldContainSingle<LoadCurrentLevel>()
                    .ShouldContainSingle<ShowPauseButton>();
            });
    }
}