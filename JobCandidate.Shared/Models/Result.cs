using System.Text.Json.Serialization;

namespace JobCandidate.Shared.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public List<string> Errors { get; private set; }

    public int StatusCode { get; private set; }

    public Result(T data, int statusCode = 200)
    {
        IsSuccess = true;
        Data = data;
        StatusCode = statusCode;
        Errors = new List<string>();
    }

    public Result(List<string> errors, int statusCode = 400)
    {
        IsSuccess = false;
        Errors = errors;
        StatusCode = statusCode;
        Data = default;
    }

    public static Result<T> Success(T data, int statusCode = 200) =>
        new Result<T>(data, statusCode);
 
    public static Result<T> Failure(List<string> errors, int statusCode = 400) =>
        new Result<T>(errors, statusCode);
}
