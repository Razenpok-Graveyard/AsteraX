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
                throw new Exception($"Contract failed: {failMessage}");
            }
        }
    }
}