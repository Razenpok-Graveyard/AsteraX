using Razensoft.Mediator;

namespace AsteraX.Application.UI.Requests
{
    public class ShowAchievementPopup : IAsyncRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}