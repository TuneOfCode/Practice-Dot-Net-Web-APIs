using LearnIndentityAndAuthorization.Controllers.Responses;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;

namespace LearnIndentityAndAuthorization.Exceptions;

public static class HandlerException
{
    /// <summary>
    /// Xử lý ngoại lệ của máy chủ
    /// </summary>
    /// <param name="app"></param>
    public static void HandleExceptionAsync(this WebApplication app)
    {
        app.UseExceptionHandler
        (
            appError => appError.Run(async context =>
            {
                int statusCode = StatusCodes.Status500InternalServerError;
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    await context.Response.WriteAsJsonAsync(new ErrorResponse()
                    {
                        Title = contextFeature.Error.Message,
                        Status = statusCode,
                        Type = ReasonPhrases.GetReasonPhrase(statusCode),
                    });
                }
            })
        );
    }
}