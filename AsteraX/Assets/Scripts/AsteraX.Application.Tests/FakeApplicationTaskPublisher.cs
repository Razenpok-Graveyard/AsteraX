using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Application;
using Cysharp.Threading.Tasks;
using FluentAssertions;

namespace AsteraX.Application.Tests
{
    public class FakeApplicationTaskPublisher : IApplicationTaskPublisher
    {
        private readonly List<object> _publishedTasks = new List<object>();

        public void Publish<T>(T task) where T : IApplicationTask
        {
            _publishedTasks.Add(task);
        }

        public UniTask PublishAsync<T>(T task, CancellationToken ct = default) where T : IAsyncApplicationTask
        {
            _publishedTasks.Add(task);
            return UniTask.CompletedTask;
        }

        public FakeApplicationTaskPublisher ShouldContainSingle<T>(Action<T> validate = null)
        {
            _publishedTasks.Should().ContainSingle(task => task is T);
            if (validate != null)
            {
                var task = _publishedTasks.OfType<T>().Single();
                validate(task);
            }
            return this;
        }
    }
}