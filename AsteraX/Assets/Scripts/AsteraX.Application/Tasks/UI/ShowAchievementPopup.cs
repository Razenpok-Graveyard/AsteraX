using Common.Application;

namespace AsteraX.Application.Tasks.UI
{
    public class ShowAchievementPopup : IAsyncApplicationTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}