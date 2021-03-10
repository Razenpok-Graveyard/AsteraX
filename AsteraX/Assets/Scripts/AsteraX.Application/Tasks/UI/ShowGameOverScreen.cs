using Common.Application;

namespace AsteraX.Application.Tasks.UI
{
    public class ShowGameOverScreen : IAsyncApplicationTask
    {
        public int Level { get; set; }
        public int Score { get; set; }
    }
}