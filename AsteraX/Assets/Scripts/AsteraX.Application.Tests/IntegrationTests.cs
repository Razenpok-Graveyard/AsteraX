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
        
        protected void VerifyNotification<TNotification>(Action<TNotification> validate = null)
            where TNotification : INotification
        {
            Mediator.VerifyNotification(validate);
        }

        protected void VerifyRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IRequest
        {
            Mediator.VerifyRequest(validate);
        }

        protected void VerifyForgetRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IAsyncRequest
        {
            Mediator.VerifyForgetRequest(validate);
        }

        protected void VerifyAsyncRequest<TRequest>(Action<TRequest> validate = null)
            where TRequest : IAsyncRequest
        {
            Mediator.VerifyAsyncRequest(validate);
        }

        protected void CompleteMediatorVerification()
        {
            Mediator.CompleteVerification();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_saveFilePath);
        }
    }
}