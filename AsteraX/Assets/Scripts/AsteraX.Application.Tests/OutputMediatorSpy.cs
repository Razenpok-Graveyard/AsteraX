using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using FluentAssertions.Execution;
using Razensoft.Mediator;

namespace AsteraX.Application.Tests
{
    public class OutputMediatorSpy : IOutputMediator
    {
        private readonly Queue<RequestBucket> _buckets = new Queue<RequestBucket>();
        private RequestBucket _currentBucket;

        public OutputMediatorSpy()
        {
            _currentBucket = new RequestBucket();
            _buckets.Enqueue(_currentBucket);
        }
        
        public void Publish<TNotification>(TNotification notification)
            where TNotification : INotification
        {
            _currentBucket.Notifications.Add(notification);
        }

        public void Send<TRequest>(TRequest request) where TRequest : IRequest
        {
            _currentBucket.Requests.Add(request);
        }

        public UniTask AsyncSend<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IAsyncRequest
        {
            _currentBucket.AsyncRequest = request;
            _currentBucket = new RequestBucket();
            _buckets.Enqueue(_currentBucket);
            return UniTask.CompletedTask;
        }

        public void ForgetSend<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IAsyncRequest
        {
            _currentBucket.ForgetRequests.Add(request);
        }

        public OutputMediatorSpy VerifyNotification<TNotification>(Action<TNotification> validate = null)
            where TNotification : INotification
        {
            var nextBucket = _buckets.Peek();
            var notification = nextBucket.Notifications.OfType<TNotification>().FirstOrDefault();
            Execute.Assertion
                .ForCondition(notification != null)
                .FailWith($"Expected a notification of type {typeof(TNotification).FullName} to be published before any async request or end of operation, but found none.");
            nextBucket.Notifications.Remove(notification);
            validate?.Invoke(notification);
            return this;
        }

        public OutputMediatorSpy VerifyRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IRequest
        {
            var nextBucket = _buckets.Peek();
            var request = nextBucket.Requests.OfType<TRequest>().FirstOrDefault();
            Execute.Assertion
                .ForCondition(request != null)
                .FailWith($"Expected a request of type {typeof(TRequest).FullName} to be sent before any async request or end of operation, but found none.");
            nextBucket.Requests.Remove(request);
            validate?.Invoke(request);
            return this;
        }

        public OutputMediatorSpy VerifyForgetRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IAsyncRequest
        {
            var nextBucket = _buckets.Peek();
            var request = nextBucket.ForgetRequests.OfType<TRequest>().FirstOrDefault();
            Execute.Assertion
                .ForCondition(request != null)
                .FailWith($"Expected an async request of type {typeof(TRequest).FullName} to be forget-sent before any async request or end of operation, but found none.");
            nextBucket.ForgetRequests.Remove(request);
            validate?.Invoke(request);
            return this;
        }

        public OutputMediatorSpy VerifyAsyncRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IAsyncRequest
        {
            var nextBucket = _buckets.Peek();
            var request = nextBucket.AsyncRequest;
            Execute.Assertion
                .ForCondition(request is TRequest)
                .FailWith($"Expected an async request of type {typeof(TRequest).FullName} to be sent before end of operation, but found none.");
            var unhandledItems = new List<Type>();
            unhandledItems.AddRange(nextBucket.Requests.Select(t => t.GetType()));
            unhandledItems.AddRange(nextBucket.ForgetRequests.Select(t => t.GetType()));
            unhandledItems.AddRange(nextBucket.Notifications.Select(t => t.GetType()));
            var unhandledItemNames = string.Join(", ", unhandledItems.Select(t => t.Name));
            Execute.Assertion
                .ForCondition(unhandledItems.Count == 0)
                .FailWith($"Expected mediator to have no unhandled items before async request of type {typeof(TRequest).FullName} is sent, but found {unhandledItems.Count} unhandled items: {unhandledItemNames}.");
            nextBucket.AsyncRequest = null;
            _buckets.Dequeue();
            return this;
        }

        public void Complete()
        {
            var unhandledItems = new List<Type>();
            while (_buckets.Any())
            {
                var nextBucket = _buckets.Dequeue();
                unhandledItems.AddRange(nextBucket.Requests.Select(t => t.GetType()));
                unhandledItems.AddRange(nextBucket.ForgetRequests.Select(t => t.GetType()));
                unhandledItems.AddRange(nextBucket.Notifications.Select(t => t.GetType()));
                if (nextBucket.AsyncRequest != null)
                {
                    unhandledItems.Add(nextBucket.AsyncRequest.GetType());
                }
            }

            var unhandledItemNames = string.Join(", ", unhandledItems.Select(t => t.Name));
            Execute.Assertion
                .ForCondition(unhandledItems.Count == 0)
                .FailWith($"Expected mediator to have no unhandled items, but found {unhandledItems.Count} unhandled items: {unhandledItemNames}.");
        }

        private class RequestBucket
        {
            public List<IRequest> Requests { get; } = new List<IRequest>();
            public List<IAsyncRequest> ForgetRequests { get; } = new List<IAsyncRequest>();
            public List<INotification> Notifications { get; } = new List<INotification>();
            public IAsyncRequest AsyncRequest { get; set; }
        }
    }
}