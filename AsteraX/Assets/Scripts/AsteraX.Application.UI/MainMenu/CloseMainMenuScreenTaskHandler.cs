using AsteraX.Application.UI.Requests;
using Common.Application.Unity;

namespace AsteraX.Application.UI.MainMenu
{
    public class CloseMainMenuScreenTaskHandler : OutputRequestHandler<HideMainMenuScreen>
    {
        protected override void Handle(HideMainMenuScreen task)
        {
            gameObject.SetActive(false);
        }
    }
}