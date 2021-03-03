using System;

namespace Common.Functional
{
    public class ResultSuccessException : Exception
    {
        internal ResultSuccessException()
            : base(Result.Messages.ErrorIsInaccessibleForSuccess)
        {
        }
    }
}
