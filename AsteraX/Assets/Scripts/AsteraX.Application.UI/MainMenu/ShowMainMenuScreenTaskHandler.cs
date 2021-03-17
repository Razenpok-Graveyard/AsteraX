using AsteraX.Application.UI.Requests;
using Common.Application.Unity;

namespace AsteraX.Application.UI.MainMenu
{
    public class ShowMainMenuScreenTaskHandler : OutputRequestHandler<ShowMainMenuScreen>
    {
        protected override void Handle(ShowMainMenuScreen task)
        {
            gameObject.SetActive(true);
        }
    }
}