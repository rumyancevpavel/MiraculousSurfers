using System;

namespace MiraculousSurfers.Core
{
    public interface IResult<out T>
    {
        bool Success { get; }
        T Value { get; }
        Exception Error { get; }
    }

    public readonly struct Result<T> : IResult<T>
    {
        public bool Success { get; }
        public T Value { get; }
        public Exception Error { get; }

        public Result(bool success, T value, Exception error)
        {
            Success = success;
            Value = value;
            Error = error;
        }
    }

    public static class Result
    {
        public static IResult<T> Ok<T>(T value) => new Result<T>(true, value, null);
        public static IResult<T> Fail<T>(Exception error) => new Result<T>(false, default, error);
    }
}
