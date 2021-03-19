using AsteraX.Domain.Game;
using Razensoft.Mediator;

namespace AsteraX.Application.UI.Requests
{
    public class ShowLoadingScreen : IAsyncRequest
    {
        public long Id { get; set; }
        public int Asteroids { get; set; }
        public int Children { get; set; }

        public static ShowLoadingScreen Create(Level level)
        {
            return new ShowLoadingScreen
            {
                Id = level.Id,
                Asteroids = level.AsteroidCount,
                Children = level.AsteroidChildCount
            };
        }
    }
}