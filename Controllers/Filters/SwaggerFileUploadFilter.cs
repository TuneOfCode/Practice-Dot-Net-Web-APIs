using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LearnIndentityAndAuthorization.Controllers.Filters;

/// <summary>
/// Cấu hình cho phép upload file trong Swagger
/// </summary>
public class SwaggerFileUploadFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Kiểm tra nếu phương thức là POST hoặc PUT và có tham số là IFormFile hoặc List<IFormFile>
        var fileParameters = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(List<IFormFile>))
            .ToList();

        if (fileParameters is null || fileParameters.Count == 0)
        {
            return;
        }

        // Thêm tham số vào Request Body của Swagger
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "multipart/form-data", new OpenApiMediaType
                        {
                            // Schema = new OpenApiSchema
                            // {
                            //     Type = "object",
                            //     Properties = fileParameters.ToDictionary(
                            //         p => p.Name,
                            //         p => new OpenApiSchema
                            //         {
                            //             Type = "string",
                            //             Format = "binary",
                            //             Description = "Tập tin ảnh"
                            //         }
                            //     )
                            // }
                        }
                    }
                }
        };
    }
}

