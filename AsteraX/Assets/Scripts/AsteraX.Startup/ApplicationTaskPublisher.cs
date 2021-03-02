using System.Threading;
using AsteraX.Application.Common;
using Cysharp.Threading.Tasks;
using UniTaskPubSub;

namespace AsteraX.Startup
{
    public class ApplicationTaskPublisher : IApplicationTaskPublisher
    {
        private readonly IAsyncPublisher _asyncPublisher;

        public ApplicationTaskPublisher(IAsyncPublisher asyncPublisher)
        {
            _asyncPublisher = asyncPublisher;
        }
        
        public void Publish<T>(T task) where T : IApplicationTask
        {
            _asyncPublisher.Publish(task);
        }

        public UniTask PublishAsync<T>(T task, CancellationToken ct = default) where T : IAsyncApplicationTask
        {
            return _asyncPublisher.PublishAsync(task, ct);
        }
    }
}