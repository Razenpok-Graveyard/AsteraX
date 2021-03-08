using AsteraX.Infrastructure.Data;
using Common.Application;
using FluentAssertions;
using NUnit.Framework;
using static AsteraX.Application.Game.Levels.StartNextLevelTaskHandler;

namespace AsteraX.Application.Game.Tests
{
    public class StartNextLevelTaskHandlerTests
    {
        [Test]
        public void Starting_new_level()
        {
            const int asteroidCount = 3;
            var levelRepository = new LevelRepository();
            var gameSessionRepository = new GameSessionRepository();
            var taskPublisher = new FakeApplicationTaskPublisher();
            IRequestHandler<Command> sut = new CommandHandler(
                levelRepository,
                gameSessionRepository,
                taskPublisher
            );
            var command = new Command();

            sut.Handle(command);

            taskPublisher
                .ShouldContainSingle<SpawnAsteroids>(task =>
                {
                    task.Asteroids.Count.Should().Be(asteroidCount);
                });
        }
    }
}