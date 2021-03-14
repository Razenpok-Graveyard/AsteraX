using System.Threading;
using Common.Application;
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

        public UniTask AsyncPublish<T>(T task, CancellationToken ct) where T : IAsyncApplicationTask
        {
            return _asyncPublisher.PublishAsync(task, ct);
        }

        public void ForgetPublish<T>(T task, CancellationToken ct) where T : IAsyncApplicationTask
        {
            _asyncPublisher.PublishAsync(task, ct).Forget();
        }
    }
}