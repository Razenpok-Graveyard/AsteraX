using AsteraX.Application.UI.Tasks;
using Common.Application.Unity;

namespace AsteraX.Application.UI.MainMenu
{
    public class ShowMainMenuScreenTaskHandler : ApplicationTaskHandler<ShowMainMenuScreen>
    {
        protected override void Handle(ShowMainMenuScreen task)
        {
            gameObject.SetActive(true);
        }
    }
}