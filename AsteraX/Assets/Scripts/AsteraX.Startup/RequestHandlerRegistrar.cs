using System;
using System.Linq;
using System.Reflection;
using Common.Application;
using UnityEditorInternal;
using UnityEngine;
using VContainer;

namespace AsteraX.Startup
{
    public static class RequestHandlerRegistrar
    {
        public static void RegisterRequestHandlers(this IContainerBuilder builder, AssemblyDefinitionAsset[] assemblies)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembliesToScan = assemblies
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
            var baseRequestHandlerType = typeof(IBaseInputRequestHandler);
            var requestHandlerTypes = new[]
            {
                typeof(IInputRequestHandler<>),
                typeof(IInputRequestHandler<,>),
                typeof(IAsyncInputRequestHandler<>),
                typeof(IAsyncInputRequestHandler<,>)
            };
            foreach (var type in assembly.GetTypes())
            {
                if (!baseRequestHandlerType.IsAssignableFrom(type))
                {
                    continue;
                }

                var registration = builder.Register(type, Lifetime.Transient);

                foreach (var @interface in type.GetInterfaces())
                {
                    if (!@interface.IsGenericType)
                    {
                        continue;
                    }

                    var genericTypeDefinition = @interface.GetGenericTypeDefinition();
                    if (requestHandlerTypes.Contains(genericTypeDefinition))
                    {
                        registration.As(@interface);
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