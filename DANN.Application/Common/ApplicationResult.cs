namespace DANN.Application.Common;

public class ApplicationResult<T>
{
    public bool IsSuccess { get; }

    public string Error { get; }

    public T? Value { get; }

    private ApplicationResult(
        bool isSuccess,
        T? value,
        string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static ApplicationResult<T> Success(T value)
    {
        return new ApplicationResult<T>(
            true,
            value,
            string.Empty);
    }

    public static ApplicationResult<T> Failure(string error)
    {
        return new ApplicationResult<T>(
            false,
            default,
            error);
    }
}
