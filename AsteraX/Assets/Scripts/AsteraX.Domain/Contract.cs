using System;
using JetBrains.Annotations;

namespace AsteraX.Domain
{
    public static class Contract
    {
        [AssertionMethod]
        public static void Requires(
            [AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition,
            [NotNull] string failMessage)
        {
            if (!condition)
            {
                throw new ContractException($"Contract failed: {failMessage}");
            }
        }
    }

    public class ContractException : Exception
    {    
        public ContractException(string message) : base(message) { }
    }
}