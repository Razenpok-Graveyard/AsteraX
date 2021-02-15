using JetBrains.Annotations;
using VContainer;
using VContainer.Unity;

namespace AsteraX.Infrastructure
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        protected override void Configure([NotNull] IContainerBuilder builder)
        {
            builder.RegisterContainer();
        }
    }
}