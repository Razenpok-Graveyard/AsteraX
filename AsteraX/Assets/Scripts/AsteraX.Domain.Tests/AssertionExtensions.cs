using System;
using FluentAssertions.Specialized;

namespace AsteraX.Domain.Tests
{
    public static class AssertionExtensions
    {
        public static ExceptionAssertions<Exception> Throw<TDelegate>(
            this DelegateAssertions<TDelegate> delegateAssertions,
            string because = "",
            params object[] becauseArgs)
            where TDelegate : Delegate
        {
            return delegateAssertions.Throw<Exception>(because, becauseArgs);
        }
    }
}