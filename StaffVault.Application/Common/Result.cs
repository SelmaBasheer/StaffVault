namespace StaffVault.Application.Common
{
    public sealed record Error(string Field, string Message);

    public sealed class Result
    {
        private Result(bool isSuccess, IReadOnlyList<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<Error> Errors { get; }

        public static Result Success() => new Result(true, Array.Empty<Error>());

        public static Result Failure(params Error[] errors) => new Result(false, errors);

        public static Result Failure(IEnumerable<Error> errors) =>
            new Result(false, errors.ToList());
    }
}
