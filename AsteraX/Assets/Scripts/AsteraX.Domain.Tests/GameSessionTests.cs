﻿using System;
using System.Linq;
using AsteraX.Domain.Game;
using FluentAssertions;
using NUnit.Framework;

namespace AsteraX.Domain.Tests
{
    public class GameSessionTests
    {
        [Test]
        public void Game_session_cannot_be_created_with_negative_jumps()
        {
            const int initialJumps = -1;

            Func<GameSession> act = () => new GameSession(initialJumps);

            act.Should().Throw("Number of jumps is negative");
        }

        [Test]
        public void Game_session_cannot_be_created_with_negative_high_score()
        {
            const int initialJumps = 1;
            const int highScore = -1;

            Func<GameSession> act = () => new GameSession(initialJumps, highScore);

            act.Should().Throw("High score is negative");
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_destroys_player_ship()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            session.IsPlayerAlive.Should().BeFalse();
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_destroys_asteroid()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            session.IsAsteroidAlive(asteroid.Id).Should().BeFalse();
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_spawns_child_asteroids()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            foreach (var child in asteroid.Children)
            {
                session.IsAsteroidAlive(child.Id).Should().BeTrue();
            }
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_decreases_jumps()
        {
            const int initialJumps = 3;
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids(initialJumps);

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            const int expectedJumps = 2;
            session.Jumps.Should().Be(expectedJumps);
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_doesnt_decrease_jumps_below_zero()
        {
            const int initialJumps = 0;
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids(initialJumps);

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            const int expectedJumps = 0;
            session.Jumps.Should().Be(expectedJumps);
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_causes_game_over_when_there_are_no_jumps_remaining()
        {
            const int initialJumps = 0;
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids(initialJumps);

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            session.IsGameOver.Should().BeTrue();
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_doesnt_cause_game_over_when_there_are_jumps_remaining()
        {
            const int initialJumps = 3;
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids(initialJumps);

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            session.IsGameOver.Should().BeFalse();
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_makes_high_score_equals_current_score_when_there_are_no_jumps_remaining_and_high_score_was_beaten()
        {
            const int initialJumps = 0;
            const int highScore = 1;
            var session = new GameSession(initialJumps, highScore);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroids = session.GetAsteroids();
            var killedAsteroid = asteroids.First();
            var killerAsteroid = asteroids.Last();
            session.CollideAsteroidWithBullet(killedAsteroid.Id);
            
            session.CollideAsteroidWithPlayerShip(killerAsteroid.Id);

            session.HighScore.Should().Be(session.Score);
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_doesnt_increase_score()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            const int expectedScore = 0;
            session.Score.Should().Be(expectedScore);
        }

        [Test]
        public void Collision_of_asteroid_and_player_ship_cannot_be_done_when_player_is_dead()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();
            var secondAsteroid = session.GetAsteroids().Skip(1).First();
            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            Action act = () => session.CollideAsteroidWithPlayerShip(secondAsteroid.Id);

            act.Should().Throw();
        }

        [Test]
        public void Collision_of_asteroid_and_bullet_destroys_asteroid()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();

            session.CollideAsteroidWithBullet(asteroid.Id);

            session.IsAsteroidAlive(asteroid.Id).Should().BeFalse();
        }

        [Test]
        public void Collision_of_asteroid_and_bullet_spawns_child_asteroids()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();

            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            foreach (var child in asteroid.Children)
            {
                session.IsAsteroidAlive(child.Id).Should().BeTrue();
            }
        }

        [Test]
        public void Collision_of_asteroid_and_bullet_increases_score_by_asteroid_score()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();

            session.CollideAsteroidWithBullet(asteroid.Id);

            var expectedScore = asteroid.Score;
            session.Score.Should().Be(expectedScore);
        }

        [Test]
        public void Collision_of_asteroid_and_bullet_beats_high_score_if_new_score_is_higher_than_active_high_score()
        {
            const int initialJumps = 3;
            const int highScore = 1;
            var session = new GameSession(initialJumps, highScore);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroids = session.GetAsteroids();

            foreach (var asteroid in asteroids)
            {
                session.CollideAsteroidWithBullet(asteroid.Id);
            }

            session.HasBeatenHighScore.Should().BeTrue();
        }

        [Test]
        public void Player_is_alive_after_respawn()
        {
            var (session, asteroid) = CreateGameSessionWithTwoAsteroids();
            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            session.RespawnPlayer();

            session.IsPlayerAlive.Should().BeTrue();
        }

        [Test]
        public void Player_is_alive_after_restart()
        {
            const int initialJumps = 0;
            var session = new GameSession(initialJumps);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroid = session.GetAsteroids().First();
            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            session.Restart();

            session.IsPlayerAlive.Should().BeTrue();
        }

        [Test]
        public void Game_session_restart_resets_jumps()
        {
            const int initialJumps = 1;
            var session = new GameSession(initialJumps);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroid = session.GetAsteroids().First();
            var secondAsteroid = session.GetAsteroids().Skip(1).First();
            session.CollideAsteroidWithPlayerShip(asteroid.Id);
            session.RespawnPlayer();
            session.CollideAsteroidWithPlayerShip(secondAsteroid.Id);

            session.Restart();

            session.Jumps.Should().Be(1);
        }

        [Test]
        public void Game_session_restart_resets_score()
        {
            const int initialJumps = 0;
            var session = new GameSession(initialJumps);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroid = session.GetAsteroids().First();
            var secondAsteroid = session.GetAsteroids().Skip(1).First();
            session.CollideAsteroidWithBullet(asteroid.Id);
            session.CollideAsteroidWithPlayerShip(secondAsteroid.Id);

            session.Restart();

            session.Score.Should().Be(0);
        }

        [Test]
        public void Game_session_has_not_beaten_high_score_after_reset()
        {
            const int initialJumps = 0;
            var session = new GameSession(initialJumps);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroid = session.GetAsteroids().First();
            var secondAsteroid = session.GetAsteroids().Skip(1).First();
            session.CollideAsteroidWithBullet(asteroid.Id);
            session.CollideAsteroidWithPlayerShip(secondAsteroid.Id);

            session.Restart();

            session.HasBeatenHighScore.Should().BeFalse();
        }

        [Test]
        public void Game_session_is_not_over_after_restart()
        {
            const int initialJumps = 0;
            var session = new GameSession(initialJumps);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroid = session.GetAsteroids().First();
            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            session.Restart();

            session.IsGameOver.Should().BeFalse();
        }

        [Test]
        public void Game_session_is_not_playing_a_level_after_restart()
        {
            const int initialJumps = 0;
            var session = new GameSession(initialJumps);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroid = session.GetAsteroids().First();
            session.CollideAsteroidWithPlayerShip(asteroid.Id);

            session.Restart();

            session.IsPlayingLevel.Should().BeFalse();
        }

        private static (GameSession, Asteroid) CreateGameSessionWithTwoAsteroids()
        {
            const int initialJumps = 3;
            return CreateGameSessionWithTwoAsteroids(initialJumps);
        }

        private static (GameSession, Asteroid) CreateGameSessionWithTwoAsteroids(int jumps)
        {
            var session = new GameSession(jumps);
            var level = CreateLevelWithTwoAsteroids();
            session.StartLevel(level);
            var asteroid = session.GetAsteroids().First();
            return (session, asteroid);
        }

        private static Level CreateLevelWithTwoAsteroids()
            => new Level(1, 2, 3);
    }
}