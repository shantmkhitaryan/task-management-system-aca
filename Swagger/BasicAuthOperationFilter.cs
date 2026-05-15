using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace task_management_system_aca.Swagger
{
    public class BasicAuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var path = context.ApiDescription.RelativePath.ToLowerInvariant();
            if (path.Contains("auth/register") || path.Contains("auth/login"))
            {
                operation.Security = [];
            }
        }
    }
}
