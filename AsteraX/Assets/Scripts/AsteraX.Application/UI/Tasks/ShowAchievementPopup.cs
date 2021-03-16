using Common.Application;

namespace AsteraX.Application.UI.Tasks
{
    public class ShowAchievementPopup : IAsyncApplicationTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}