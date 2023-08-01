using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace LearnIndentityAndAuthorization.Controllers.Responses;

public class ErrorResponse : IActionResult
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int Status { get; set; }
    public Guid TraceId { get; set; }

    public ErrorResponse(string? title = null!, int status = StatusCodes.Status400BadRequest, string? type = null)
    {
        Title = title;
        Status = status;
        Type = type ?? ReasonPhrases.GetReasonPhrase(Status);
        TraceId = Guid.NewGuid();
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        var response = new
        {
            Type,
            Title,
            Status,
            TraceId
        };
        context.HttpContext.Response.StatusCode = Status;
        return new JsonResult(response).ExecuteResultAsync(context);
    }
}