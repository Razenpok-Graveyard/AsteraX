using System;
using System.Collections.Generic;
using System.Threading;
using Common.Application;
using Cysharp.Threading.Tasks;
using FluentAssertions;

namespace AsteraX.Application.UI.Tests
{
    public class FakeApplicationTaskPublisher : IApplicationTaskPublisher
    {
        private readonly Queue<object> _publishedTasks = new Queue<object>();

        public void PublishTask<T>(T task) where T : IApplicationTask
        {
            _publishedTasks.Enqueue(task);
        }

        public UniTask PublishAsyncTask<T>(T task, CancellationToken ct = default) where T : IAsyncApplicationTask
        {
            _publishedTasks.Enqueue(task);
            return UniTask.CompletedTask;
        }

        public FakeApplicationTaskPublisher Consume<T>(Action<T> validate = null)
        {
            var task = _publishedTasks.Dequeue();
            task.Should().BeOfType<T>();
            validate?.Invoke((T) task);
            return this;
        }
    }
}