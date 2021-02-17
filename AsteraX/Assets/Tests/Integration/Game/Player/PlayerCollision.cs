using System.Threading;
using AsteraX.Application.Game;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.Game.Player;
using AsteraX.Infrastructure.Data;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace AsteraX.Tests.Integration.Game.Player
{
    public class PlayerCollision
    {
        [Test]
        public void Should_destroy_collided_objects()
        {
            var repository = new GameSessionRepository();
            var gameField = new FakeGameField();
            
            var sut = new PlayerShipCollisionController.CommandHandler(repository, gameField);

            var go = new GameObject();
            var asteroid = go.AddComponent<Asteroid>();
            var command = new PlayerShipCollisionController.Command
            {
                Asteroid = asteroid
            };
            sut.Handle(command, CancellationToken.None);

            gameField.DestroyedPlayerShip.Should().BeTrue();
            gameField.DestroyedAsteroid.Should().Be(asteroid);
            
            Object.Destroy(go);
        }
    }

    public class FakeGameField : IGameField
    {
        public Asteroid DestroyedAsteroid { get; private set; }
        public bool DestroyedPlayerShip { get; private set; }

        public void Destroy(Asteroid asteroid)
        {
            DestroyedAsteroid = asteroid;
        }

        public void DestroyPlayerShip()
        {
            DestroyedPlayerShip = true;
        }
    }
}