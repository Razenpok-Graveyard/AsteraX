using Common.Application;

namespace AsteraX.Application.UI.Requests
{
    public class ShowHighScorePopup : IAsyncRequest
    {
        public int HighScore { get; set; }
    }
}