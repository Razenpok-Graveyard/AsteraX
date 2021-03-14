using AsteraX.Domain.Game;
using Common.Application;

namespace AsteraX.Application.Tasks.UI
{
    public class ShowLoadingScreen : IAsyncApplicationTask
    {
        public int Id { get; set; }
        public int Asteroids { get; set; }
        public int Children { get; set; }

        public static ShowLoadingScreen Create(Level level)
        {
            return new ShowLoadingScreen
            {
                Id = (int) level.Id,
                Asteroids = level.AsteroidCount,
                Children = level.AsteroidChildCount
            };
        }
    }
}