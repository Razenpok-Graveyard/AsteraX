using AsteraX.Infrastructure.Data;
using Common.Application;
using Common.Application.Unity;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AsteraX.Startup
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private LevelSettings _levelSettings;
        [SerializeField] private AssemblyDefinitionAsset[] _requestHandlerAssemblies;

        protected override void Configure([NotNull] IContainerBuilder builder)
        {
            builder.RegisterContainer();
            builder.RegisterRequestHandlers(_requestHandlerAssemblies);

            builder.RegisterInstance(OutputMediator.Default).As<IOutputMediator>();

            builder.RegisterInstance(_levelSettings);
            builder.Register<GameSessionSettings>(Lifetime.Singleton);
            builder.Register<LevelRepository>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameSessionRepository>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.RegisterBuildCallback(InjectAllMonoBehaviours);
        }

        private static void InjectAllMonoBehaviours(IObjectResolver container)
        {
            var monoBehaviours = FindObjectsOfType<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours)
            {
                container.Inject(monoBehaviour);
            }
        }
    }
}