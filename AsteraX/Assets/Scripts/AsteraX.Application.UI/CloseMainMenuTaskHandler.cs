namespace AsteraX.Application.UI
{
    public class CloseMainMenuTaskHandler : ApplicationTaskHandler<CloseMainMenu>
    {
        protected override void Handle(CloseMainMenu task)
        {
            gameObject.SetActive(false);
        }
    }
}