using System.Threading;
using Cysharp.Threading.Tasks;

namespace Common.Application
{
    public interface IApplicationTaskPublisher
    {
        void PublishTask<T>(T task) where T : IApplicationTask;
        UniTask PublishAsyncTask<T>(T task, CancellationToken ct = default) where T : IAsyncApplicationTask;
    }
}