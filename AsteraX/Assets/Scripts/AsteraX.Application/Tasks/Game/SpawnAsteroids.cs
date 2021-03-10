using System.Collections.Generic;
using Common.Application;

namespace AsteraX.Application.Tasks.Game
{
    public class SpawnAsteroids : IApplicationTask
    {
        public List<AsteroidDto> Asteroids { get; set; } = new List<AsteroidDto>();

        public class AsteroidDto
        {
            public long Id { get; set; }
            public int Size { get; set; }
            public List<AsteroidDto> Children { get; set; } = new List<AsteroidDto>();
        }
    }
}