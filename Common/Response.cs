using System;

namespace API.Common;

public class Response<T>
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public string? Error { get; }
    public T? Data { get; }

    public Response(bool isSuccess, string? message = null, string? error = null, T? data = default)
    {
        IsSuccess = isSuccess;
        Message = message;
        Error = error;
        Data = data;
    }

    public static Response<T> Success(string? message = "", T? data = default) => new Response<T>(true, message, null, data);
    public static Response<T> Failure(string error) => new Response<T>(false,null,error,default);
}
