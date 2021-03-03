﻿namespace Common.Functional
{
    public partial struct Result
    {
        public override string ToString()
        {
            return IsSuccess ? "Success" : $"Failure({Error})";
        }
    }


    public partial struct Result<T>
    {
        public override string ToString()
        {
            return IsSuccess ? $"Success({Value})" : $"Failure({Error})";
        }
    }


    public partial struct Result<T, E>
    {
        public override string ToString()
        {
            return IsSuccess ? $"Success({Value})" : $"Failure({Error})";
        }
    }
}