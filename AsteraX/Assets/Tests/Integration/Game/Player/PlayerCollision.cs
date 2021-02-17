using AsteraX.Application.Game;
using AsteraX.Application.Game.Asteroids;
using AsteraX.Application.Game.Player;
using AsteraX.Domain.GameSession;
using AsteraX.Infrastructure;
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
            var repository = new StubGameSessionRepository();
            var gameField = new FakeGameField();
            
            var sut = new PlayerShipCollisionController.CommandHandler(repository, gameField);

            var go = new GameObject();
            var asteroid = go.AddComponent<Asteroid>();
            sut.Execute(asteroid);

            gameField.DestroyedPlayerShip.Should().BeTrue();
            gameField.DestroyedAsteroid.Should().Be(asteroid);
            
            Object.Destroy(go);
        }
    }

    public class StubGameSessionRepository : IGameSessionRepository
    {
        private readonly GameSession _gameSession = new GameSession(3);
        
        public GameSession GetCurrentSession()
        {
            return _gameSession;
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