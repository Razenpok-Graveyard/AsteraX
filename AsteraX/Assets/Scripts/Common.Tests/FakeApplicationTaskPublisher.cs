using System;
using System.Collections.Generic;
using System.Threading;
using Common.Application;
using Cysharp.Threading.Tasks;
using FluentAssertions.Execution;

namespace Common.Tests
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
            Execute.Assertion
                .ForCondition(task is T)
                .FailWith($"Expected task of type {typeof(T).FullName} to be published, but found {task.GetType().FullName}.");
            validate?.Invoke((T) task);
            return this;
        }

        public void Complete()
        {
            Execute.Assertion
                .ForCondition(_publishedTasks.Count == 0)
                .FailWith($"Expected task publisher to be empty, but found {_publishedTasks.Count} unverified tasks.");
        }
    }
}