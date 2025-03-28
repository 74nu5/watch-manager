namespace Watch.Manager.Common;

/// <summary>
///     Represents the result of an API call, including the result data, error type, and error message.
/// </summary>
/// <typeparam name="TResult">The type of the result data.</typeparam>
public sealed record ApiResult<TResult>(TResult? Result, ApiResultErrorType? ApiResultErrorType = null, string? Error = null)
{
    /// <summary>
    ///     Creates a successful <see cref="ApiResult{TResult}" /> with the specified result data.
    /// </summary>
    /// <param name="result">The result data.</param>
    /// <returns>A successful <see cref="ApiResult{TResult}" />.</returns>
    public static ApiResult<TResult> Success(TResult result)
        => new(result);

    /// <summary>
    ///     Creates a failed <see cref="ApiResult{TResult}" /> with the specified error type.
    /// </summary>
    /// <param name="apiResultErrorType">The type of the error.</param>
    /// <returns>A failed <see cref="ApiResult{TResult}" />.</returns>
    public static ApiResult<TResult> Failure(ApiResultErrorType apiResultErrorType)
        => new(default, apiResultErrorType);

    /// <summary>
    ///     Creates a failed <see cref="ApiResult{TResult}" /> with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed <see cref="ApiResult{TResult}" />.</returns>
    public static ApiResult<TResult> Failure(string error)
        => new(default, null, error);

    /// <summary>
    ///     Creates a failed <see cref="ApiResult{TResult}" /> with the specified error type and error message.
    /// </summary>
    /// <param name="apiResultErrorType">The type of the error.</param>
    /// <param name="error">The error message.</param>
    /// <returns>A failed <see cref="ApiResult{TResult}" />.</returns>
    public static ApiResult<TResult> Failure(ApiResultErrorType apiResultErrorType, string error)
        => new(default, apiResultErrorType, error);
}
