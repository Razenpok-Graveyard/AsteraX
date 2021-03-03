using System;
using System.Threading;
using Common.Application;
using Cysharp.Threading.Tasks;
using UniTaskPubSub;

namespace AsteraX.Startup
{
    public class ApplicationTaskSubscriber : IApplicationTaskSubscriber
    {
        private readonly IAsyncSubscriber _asyncSubscriber;

        public ApplicationTaskSubscriber(IAsyncSubscriber asyncSubscriber)
        {
            _asyncSubscriber = asyncSubscriber;
        }
        
        public IDisposable Subscribe<T>(Action<T> handler) where T : IApplicationTask
        {
            return _asyncSubscriber.Subscribe(handler);
        }

        public IDisposable Subscribe<T>(
            Func<T, CancellationToken, UniTask> handler,
            CancellationToken ct = default)
            where T : IAsyncApplicationTask
        {
            return _asyncSubscriber.Subscribe(handler, ct);
        }
    }
}