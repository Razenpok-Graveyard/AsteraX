using AsteraX.Application.Game;
using AsteraX.Application.Game.Tests;
using Common.Application;
using NUnit.Framework;
using static AsteraX.Application.UI.StartButtonController;

namespace AsteraX.Application.UI.Tests
{
    public class StartButtonControllerTests
    {
        [Test]
        public void Pressing_start_button()
        {
            var taskPublisher = new FakeApplicationTaskPublisher();
            IRequestHandler<Command> sut = new CommandHandler(taskPublisher);
            var command = new Command();

            sut.Handle(command);

            taskPublisher
                .ShouldContainSingle<CloseMainMenu>()
                .ShouldContainSingle<StartNextLevel>();
        }
    }
}