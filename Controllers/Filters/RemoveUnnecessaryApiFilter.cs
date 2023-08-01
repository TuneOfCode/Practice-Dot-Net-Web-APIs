using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LearnIndentityAndAuthorization.Controllers.Filters;

/// <summary>
/// Loại bỏ các route không cần yêu cầu bảo mật
/// </summary>
public partial class RemoveUnnecessaryApiFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
        {
            return;
        }

        operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{
                        }
                    }
                }
            };
        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
    }
}