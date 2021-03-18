using AsteraX.Application.Achievements;
using AsteraX.Infrastructure;
using Common.Application;
using Common.Application.Unity;
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
        [SerializeField] private LevelSettings _levelSettings;
        [SerializeField] private AssemblyDefinitionAsset[] _requestHandlerAssemblies;

        protected override void Configure([NotNull] IContainerBuilder builder)
        {
            builder.RegisterContainer();
            builder.RegisterRequestHandlers(_requestHandlerAssemblies);

            var messageBus = new MessageBus(AsyncMessageBus.Default);
            var mediator = new OutputMediator(messageBus);
            MediatorMonoBehaviourExtensions.Mediator = mediator;
            builder.RegisterInstance(mediator).As<IOutputMediator>().AsSelf();

            builder.RegisterInstance(_levelSettings);

            builder.Register<SaveFileProvider>(Lifetime.Singleton)
                .WithParameter<string>(SaveFileProvider.DefaultSaveFilePath);
            builder.Register<GameSessionSettings>(Lifetime.Singleton);
            builder.Register<AchievementRepository>(Lifetime.Singleton);
            builder.Register<LevelRepository>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameSessionRepository>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterNonLazy<AchievementController>(Lifetime.Singleton);
            
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

    internal static class ContainerBuilderExtensions
    {
        public static RegistrationBuilder RegisterNonLazy<T>(
            this IContainerBuilder builder,
            Lifetime lifetime)
        {
            var registrationBuilder = builder.Register(typeof(T), lifetime);
            builder.RegisterBuildCallback(container => container.Resolve<T>());
            return registrationBuilder;
        }
    }
}