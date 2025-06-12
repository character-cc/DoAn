using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Common;

public class AppErrorResponse : ProblemDetails
{
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]> Errors { get; set; } = new();

    [JsonPropertyName("errorType")]
    public string ErrorType { get; set; } = AppErrorType.BusinessRule.ToString();


    public static AppErrorResponse From(string field, string message, AppErrorType errorType = AppErrorType.BusinessRule, string traceId = null)
    {
        return new AppErrorResponse
        {
           
            ErrorType = errorType.ToString(),
            Errors = new Dictionary<string, string[]> { [field] = new[] { message } }
        };
    }

    public static AppErrorResponse From(Dictionary<string, string[]> errors, AppErrorType errorType = AppErrorType.BusinessRule, string traceId = null)
    {
        return new AppErrorResponse
        {
            ErrorType = errorType.ToString(),
            Errors = errors
        };
    }
}

