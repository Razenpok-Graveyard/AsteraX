using System.Collections.Generic;
using System.Linq;
using AsteraX.Application.Game.Requests;
using AsteraX.Domain.Game;
using FluentAssertions.Execution;

namespace AsteraX.Application.Tests
{
    public static class ApplicationTaskExtensions
    {
        public static void ShouldBeConsistentWithLevel(this SpawnAsteroids task, Level level)
        {
            Execute.Assertion
                .ForCondition(task.Asteroids.Count == level.AsteroidCount)
                .FailWith($"Expected SpawnAsteroids task to have {level.AsteroidCount} asteroids, but it has {task.Asteroids.Count} asteroids)");
            ShouldBeConsistentWithLevel(task.Asteroids, level);
        }

        private static void ShouldBeConsistentWithLevel(List<SpawnAsteroids.AsteroidDto> asteroids, Level level)
        {
            foreach (var asteroid in asteroids.Where(a => a.Size > 1))
            {
                Execute.Assertion
                    .ForCondition(asteroid.Children.Count == level.AsteroidChildCount)
                    .FailWith($"Expected all asteroids within SpawnAsteroids task to have {level.AsteroidChildCount} child asteroids, but some asteroids have {asteroid.Children.Count} asteroids)");
                ShouldBeConsistentWithLevel(asteroid.Children, level);
            }
        }
    }
}