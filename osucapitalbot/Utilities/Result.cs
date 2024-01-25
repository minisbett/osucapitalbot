using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot.Utilities;

/// <summary>
/// Represents the result of an operation without a return value.
/// </summary>
public class Result
{
  /// <summary>
  /// The resulting error of the operation. Null if <see cref="IsSuccessful"/> is true.
  /// </summary>
  public Error? Error { get; init; }

  /// <summary>
  /// Bool whether the operation was successful.
  /// </summary>
  public bool IsSuccessful => Error is null;

  /// <summary>
  /// Bool whether the operation failed. An error can be found in <see cref="Error"/>.
  /// </summary>
  public bool IsFailure => !IsSuccessful;

  /// <summary>
  /// A result indicating the operation was successful.
  /// </summary>
  public static Result Success() => new Result();

  /// <summary>
  /// A result incidacting the operation failed, with the specified error.<br/>
  /// If no error was specified, <see cref="Error.Unspecific"/> is used as the default.
  /// </summary>
  /// <param name="error">The error.</param>
  public static Result Failure(Error error) => new Result() { Error = error };

  /// <summary>
  /// A result indicating the operation was successful, with the specified return value.
  /// </summary>
  /// <typeparam name="T">The type of the return value.</typeparam>
  /// <param name="value">The return value.</param>
  public static Result<T> Success<T>(T value) => new Result<T>(value);

  /// <summary>
  /// A result indicating the operation failed, with the specified error.
  /// </summary>
  /// <typeparam name="T">The type of the return value.</typeparam>
  /// <param name="error">The error.</param>
  public static Result<T> Failure<T>(Error error) => new Result<T>() { Error = error };
}

/// <summary>
/// Represents the result of an operation with a return value.
/// </summary>
/// <typeparam name="T">The type of the return value.</typeparam>
public class Result<T> : Result
{
  /// <summary>
  /// The return value of the operation. Null if <see cref="Result.IsFailure"/> is true.
  /// </summary>
  private readonly T? _value;

  /// <summary>
  /// The return value of the operation.
  /// </summary>
  public T Value => IsSuccessful ? _value! : throw new InvalidOperationException("Cannot access the return value of a failed result.");

  internal Result(T? value = default)
  {
    _value = value;
  }

  /// <summary>
  /// Implicitly converts the specified return value into a <see cref="Result.Success{T}(T)"/> object.
  /// </summary>
  /// <param name="value">The value.</param>
  public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// Represents the error of a <see cref="Result"/> object.
/// </summary>
public class Error
{
  /// <summary>
  /// Indicates non-specific operation failures. 
  /// </summary>
  public static readonly Error Unspecific = new Error("unspecific", "An error occured.");

  /// <summary>
  /// Indicates that an authorization attempt using OAuth credentials failed.
  /// </summary>
  public static readonly Error OAuthAuthorization = new Error("oauth_unauthorized", "Authorization using the OAuth credentials failed.");

  /// <summary>
  /// Indicates that an API is unavailable.
  /// </summary>
  public static readonly Error APIUnavailable = new Error("api_unavailable", "The API is currently unavailable.");

  /// <summary>
  /// The error code.
  /// </summary>
  public string Code { get; }

  /// <summary>
  /// The error message.
  /// </summary>
  public string Message { get; }

  private Error(string code, string message)
  {
    Code = code;
    Message = message;
  }

  /// <summary>
  /// Implicitly converts the error into a <see cref="Result.Failure(Error?)"/> object.
  /// </summary>
  /// <param name="error">The error.</param>
  public static implicit operator Result(Error error) => Result.Failure(error);

  public override string ToString()
  {
    return $"{Code} - {Message}";
  }
}
