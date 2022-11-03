using ExchangeService.Core.Infrastructure.Holders.Abstractions;
using Microsoft.OpenApi.Models;

namespace ExchangeService.Core.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SwaggerHeaderAttribute : Attribute
    {
        public List<OpenApiParameter> Parameters { get; }

        public SwaggerHeaderAttribute()
        {
            Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = nameof(IHolder.UserId),
                    In = ParameterLocation.Header,
                    Description = "User Id",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = nameof(Int32)
                    }
                }
            };
        }
    }
}