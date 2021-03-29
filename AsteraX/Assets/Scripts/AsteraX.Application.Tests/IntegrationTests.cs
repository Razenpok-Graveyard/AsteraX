using System;
using System.IO;
using AsteraX.Infrastructure;
using NUnit.Framework;
using Razensoft.Mediator;

namespace AsteraX.Application.Tests
{
    public abstract class IntegrationTests
    {
        private string _saveFilePath;
        
        [SetUp]
        public void SetUp()
        {
            var guid = Guid.NewGuid().ToString();
            _saveFilePath = UnityEngine.Application.persistentDataPath + "/" + guid;
            SaveFile = new SaveFile(_saveFilePath);
            Mediator = new OutputMediatorSpy();
        }
        
        protected SaveFile SaveFile { get; private set; }

        protected OutputMediatorSpy Mediator { get; private set; }
        
        protected void HandleNotification<TNotification>(Action<TNotification> validate = null)
            where TNotification : INotification
        {
            Mediator.HandleNotification(validate);
        }

        protected void HandleRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IRequest
        {
            Mediator.HandleRequest(validate);
        }

        protected void HandleForgetRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IAsyncRequest
        {
            Mediator.HandleForgetRequest(validate);
        }

        protected void HandleAsyncRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IAsyncRequest
        {
            Mediator.HandleAsyncRequest(validate);
        }

        protected void Complete()
        {
            Mediator.Complete();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_saveFilePath);
        }
    }
}