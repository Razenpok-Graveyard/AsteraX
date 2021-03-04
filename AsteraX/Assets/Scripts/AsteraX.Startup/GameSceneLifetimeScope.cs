using AsteraX.Infrastructure.Data;
using Common.Application;
using JetBrains.Annotations;
using UniTaskPubSub;
using UnityEditorInternal;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AsteraX.Startup
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private AssemblyDefinitionAsset[] _requestHandlerAssemblies;

        protected override void Configure([NotNull] IContainerBuilder builder)
        {
            builder.RegisterContainer();
            builder.RegisterRequestHandlers(_requestHandlerAssemblies);

            var publisher = new ApplicationTaskPublisher(AsyncMessageBus.Default);
            var subscriber = new ApplicationTaskSubscriber(AsyncMessageBus.Default);
            builder.RegisterInstance(publisher).As<IApplicationTaskPublisher>();
            builder.RegisterInstance(subscriber).As<IApplicationTaskSubscriber>();
            ApplicationTaskDispatcher.Subscriber = subscriber;

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