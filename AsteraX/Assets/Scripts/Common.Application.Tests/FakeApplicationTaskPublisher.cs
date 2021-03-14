using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using FluentAssertions.Execution;

namespace Common.Application.Tests
{
    public class FakeApplicationTaskPublisher : IApplicationTaskPublisher
    {
        private readonly Queue<TaskBucket> _buckets = new Queue<TaskBucket>();
        private TaskBucket _currentBucket;

        public FakeApplicationTaskPublisher()
        {
            _currentBucket = new TaskBucket();
            _buckets.Enqueue(_currentBucket);
        }

        public void Publish<T>(T task) where T : IApplicationTask
        {
            _currentBucket.Tasks.Add(task);
        }

        public UniTask AsyncPublish<T>(T task, CancellationToken ct = default) where T : IAsyncApplicationTask
        {
            _currentBucket.AsyncTask = task;
            _currentBucket = new TaskBucket();
            _buckets.Enqueue(_currentBucket);
            return UniTask.CompletedTask;
        }

        public void ForgetPublish<T>(T task, CancellationToken ct) where T : IAsyncApplicationTask
        {
            _currentBucket.ForgetTasks.Add(task);
        }

        public FakeApplicationTaskPublisher Consume<T>(Action<T> validate = null) where T : IApplicationTask
        {
            var nextBucket = _buckets.Peek();
            var task = nextBucket.Tasks.OfType<T>().FirstOrDefault();
            Execute.Assertion
                .ForCondition(task != null)
                .FailWith($"Expected a task of type {typeof(T).FullName} to be published before any async task or end of operation, but found none.");
            nextBucket.Tasks.Remove(task);
            validate?.Invoke(task);
            return this;
        }

        public FakeApplicationTaskPublisher ConsumeForget<T>(Action<T> validate = null) where T : IAsyncApplicationTask
        {
            var nextBucket = _buckets.Peek();
            var task = nextBucket.ForgetTasks.OfType<T>().FirstOrDefault();
            Execute.Assertion
                .ForCondition(task != null)
                .FailWith($"Expected an async task of type {typeof(T).FullName} to be forget-published before any async task or end of operation, but found none.");
            nextBucket.ForgetTasks.Remove(task);
            validate?.Invoke(task);
            return this;
        }

        public FakeApplicationTaskPublisher ConsumeAsync<T>(Action<T> validate = null) where T : IAsyncApplicationTask
        {
            var nextBucket = _buckets.Peek();
            var task = nextBucket.AsyncTask;
            Execute.Assertion
                .ForCondition(task is T)
                .FailWith($"Expected an async task of type {typeof(T).FullName} to be published before end of operation, but found none.");
            var unverifiedTaskCount = nextBucket.Tasks.Count + nextBucket.ForgetTasks.Count;
            Execute.Assertion
                .ForCondition(unverifiedTaskCount == 0)
                .FailWith($"Expected task publisher to have no pending unverified tasks before async task of type {typeof(T).FullName} is published, but found {unverifiedTaskCount} unverified tasks..");
            nextBucket.AsyncTask = null;
            _buckets.Dequeue();
            return this;
        }

        public void Complete()
        {
            var unverifiedTaskCount = 0;
            while (_buckets.Any())
            {
                var nextBucket = _buckets.Dequeue();
                unverifiedTaskCount += nextBucket.Tasks.Count;
                unverifiedTaskCount += nextBucket.ForgetTasks.Count;
                if (nextBucket.AsyncTask != null)
                {
                    unverifiedTaskCount++;
                }
            }
            Execute.Assertion
                .ForCondition(unverifiedTaskCount == 0)
                .FailWith($"Expected task publisher to be fully verified, but found {unverifiedTaskCount} unverified tasks.");
        }

        private class TaskBucket
        {
            public List<IApplicationTask> Tasks { get; } = new List<IApplicationTask>();
            public List<IAsyncApplicationTask> ForgetTasks { get; } = new List<IAsyncApplicationTask>();
            public IAsyncApplicationTask AsyncTask { get; set; }
        }
    }
}