using System.Collections.Generic;
using System.Linq;
using AsteraX.Domain.Game;
using Common.Application;

namespace AsteraX.Application.Game.Tasks
{
    public class SpawnAsteroids : IApplicationTask
    {
        public List<AsteroidDto> Asteroids { get; set; } = new List<AsteroidDto>();

        public static SpawnAsteroids Create(IEnumerable<Asteroid> asteroids)
        {
            return new SpawnAsteroids
            {
                Asteroids = AsteroidDto.FromDomain(asteroids)
            };
        }

        public class AsteroidDto
        {
            public long Id { get; set; }
            public int Size { get; set; }
            public List<AsteroidDto> Children { get; set; } = new List<AsteroidDto>();

            public static List<AsteroidDto> FromDomain(IEnumerable<Asteroid> asteroids)
            {
                return asteroids.Select(FromDomain).ToList();
            }

            private static AsteroidDto FromDomain(Asteroid asteroid)
            {
                return new AsteroidDto
                {
                    Id = asteroid.Id,
                    Size = asteroid.Size,
                    Children = FromDomain(asteroid.Children)
                };
            }
        }
    }
}