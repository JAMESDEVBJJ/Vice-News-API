using System.Diagnostics.CodeAnalysis;

namespace ViceNews.API.Services;

public enum ServiceErrorType
{
    Validation,
    NotFound,
    Conflict,
}

public sealed record ServiceError(ServiceErrorType Type, string Message, string? Field = null)
{
    public static ServiceError Validation(string field, string message) => new(ServiceErrorType.Validation, message, field);
    public static ServiceError NotFound(string message) => new(ServiceErrorType.NotFound, message);
    public static ServiceError Conflict(string message) => new(ServiceErrorType.Conflict, message);
}

public sealed class ServiceResult<T>
{
    private ServiceResult(T? value, ServiceError? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }
    public ServiceError? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Succeeded => Error is null;

    public static ServiceResult<T> Success(T value) => new(value, null);
    public static ServiceResult<T> Failure(ServiceError error) => new(default, error);

    public static implicit operator ServiceResult<T>(ServiceError error) => Failure(error);
}
