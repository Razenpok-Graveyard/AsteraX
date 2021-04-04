using System.Collections.Generic;
using AsteraX.Domain.Game;
using Razensoft.Mapper;
using Razensoft.Mediator;

namespace AsteraX.Application.Game.Requests
{
    public class SpawnAsteroids : IRequest
    {
        public List<AsteroidDto> Asteroids { get; set; } = new List<AsteroidDto>();
        
        public class Mapper : IMapper<IEnumerable<Asteroid>, SpawnAsteroids>
        {
            public static Mapper Instance { get; } = new Mapper();

            public void Map(IEnumerable<Asteroid> source, SpawnAsteroids destination)
            {
                destination.Asteroids = AsteroidDto.Mapper.Instance.MapList(source);
            }
        }

        public class AsteroidDto
        {
            public long Id { get; set; }
            public int Size { get; set; }
            public List<AsteroidDto> Children { get; set; } = new List<AsteroidDto>();
        
            public class Mapper : IMapper<Asteroid, AsteroidDto>
            {
                public static Mapper Instance { get; } = new Mapper();

                public void Map(Asteroid source, AsteroidDto destination)
                {
                    destination.Id = source.Id;
                    destination.Size = source.Size;
                    destination.Children = this.MapList(source.Children);
                }
            }
        }
    }
}