using System;
using System.Linq;
using System.Reflection;
using AsteraX.Application.Game;
using AsteraX.Application.SharedKernel;
using AsteraX.Infrastructure;
using AsteraX.Infrastructure.Data;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AsteraX.Startup
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameField _gameField;
        [SerializeField] private AssemblyDefinitionAsset[] _requestHandlerAssemblies;

        protected override void Configure([NotNull] IContainerBuilder builder)
        {
            builder.RegisterContainer();
            builder.RegisterInstance(_gameField).As<IGameField>();
            builder.Register<IGameSessionRepository, GameSessionRepository>(Lifetime.Singleton);

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembliesToScan = _requestHandlerAssemblies
                .Select(asset => FindAssembly(asset, allAssemblies));
            foreach (var assembly in assembliesToScan)
            {
                RegisterRequestHandlers(assembly, builder);
            }
        }

        private static Assembly FindAssembly(AssemblyDefinitionAsset assemblyDefinitionAsset, Assembly[] assemblies)
        {
            var data = JsonUtility.FromJson<AssemblyDefinitionAssetData>(assemblyDefinitionAsset.text);
            return assemblies.First(a => a.FullName.Split(',')[0] == data.name);
        }

        private static void RegisterRequestHandlers(Assembly assembly, IContainerBuilder builder)
        {
            var baseRequestHandlerType = typeof(IBaseRequestHandler);
            var requestHandlerType = typeof(IRequestHandler<>);
            var requestHandlerType2 = typeof(IRequestHandler<,>);
            foreach (var type in assembly.GetTypes())
            {
                if (!baseRequestHandlerType.IsAssignableFrom(type))
                {
                    continue;
                }

                var registration = builder.Register(type, Lifetime.Transient).AsSelf();
                
                foreach (var @interface in type.GetInterfaces())
                {
                    if (!@interface.IsGenericType)
                    {
                        continue;
                    }
                    var genericTypeDefinition = @interface.GetGenericTypeDefinition();
                    var isRequestHandler = genericTypeDefinition == requestHandlerType
                                           || genericTypeDefinition == requestHandlerType2;
                    if (isRequestHandler)
                    {
                        registration.As(@interface);
                        Debug.Log(type.FullName + " to " + @interface.FullName);
                    }
                }
            }
        }

        private class AssemblyDefinitionAssetData
        {
            public string name;
        }
    }
}