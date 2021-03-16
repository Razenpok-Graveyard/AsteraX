using AsteraX.Application.UI.Tasks;
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