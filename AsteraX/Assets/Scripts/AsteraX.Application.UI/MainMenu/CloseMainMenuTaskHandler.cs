using AsteraX.Application.Tasks.UI;
using Common.Application.Unity;

namespace AsteraX.Application.UI.MainMenu
{
    public class CloseMainMenuTaskHandler : ApplicationTaskHandler<CloseMainMenu>
    {
        protected override void Handle(CloseMainMenu task)
        {
            gameObject.SetActive(false);
        }
    }
}