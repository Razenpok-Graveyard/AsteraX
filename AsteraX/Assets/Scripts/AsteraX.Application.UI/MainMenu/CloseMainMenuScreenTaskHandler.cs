using AsteraX.Application.Tasks.UI;
using Common.Application.Unity;

namespace AsteraX.Application.UI.MainMenu
{
    public class CloseMainMenuScreenTaskHandler : ApplicationTaskHandler<HideMainMenuScreen>
    {
        protected override void Handle(HideMainMenuScreen task)
        {
            gameObject.SetActive(false);
        }
    }
}