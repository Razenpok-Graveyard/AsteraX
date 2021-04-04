using AsteraX.Domain.Game;
using Razensoft.Mapper;
using Razensoft.Mediator;

namespace AsteraX.Application.UI.Requests
{
    public class ShowLoadingScreen : IAsyncRequest
    {
        public long Id { get; set; }
        public int Asteroids { get; set; }
        public int Children { get; set; }

        public class Mapper : IMapper<Level, ShowLoadingScreen>
        {
            public static Mapper Instance { get; } = new Mapper();

            public void Map(Level source, ShowLoadingScreen destination)
            {
                destination.Id = source.Id;
                destination.Asteroids = source.AsteroidCount;
                destination.Children = source.AsteroidChildCount;
            }
        }
    }
}