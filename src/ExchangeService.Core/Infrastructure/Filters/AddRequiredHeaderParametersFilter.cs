using ExchangeService.Core.Infrastructure.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ExchangeService.Core.Infrastructure.Filters;

public class AddRequiredHeaderParametersFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        // var attribute = context.MethodInfo.GetCustomAttribute<SwaggerHeaderAttribute>() ??
        //                 context.MethodInfo.DeclaringType!.GetCustomAttribute<SwaggerHeaderAttribute>();
        //
        // if (attribute == null) return;

        var attribute = new SwaggerHeaderAttribute();

        foreach (var parameter in attribute.Parameters)
            operation.Parameters.Add(parameter);
    }
}