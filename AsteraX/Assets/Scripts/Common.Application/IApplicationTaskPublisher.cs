using System.Threading;
using Cysharp.Threading.Tasks;

namespace Common.Application
{
    public interface IApplicationTaskPublisher
    {
        void Publish<T>(T task) where T : IApplicationTask;
        UniTask AsyncPublish<T>(T task, CancellationToken ct) where T : IAsyncApplicationTask;
        void ForgetPublish<T>(T task, CancellationToken ct = default) where T : IAsyncApplicationTask;
    }
}