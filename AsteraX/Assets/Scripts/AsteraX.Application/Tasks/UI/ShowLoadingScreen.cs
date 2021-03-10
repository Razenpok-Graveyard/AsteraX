using Common.Application;

namespace AsteraX.Application.Tasks.UI
{
    public class ShowLoadingScreen : IAsyncApplicationTask
    {
        public int Id { get; set; }
        public int Asteroids { get; set; }
        public int Children { get; set; }
    }
}