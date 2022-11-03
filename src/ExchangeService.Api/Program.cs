using System.Reflection;
using ExchangeService.Api.Filters;
using ExchangeService.Core.Infrastructure.Filters;
using ExchangeService.Infrastructure;
using FluentValidation;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.

services.AddControllers(options =>
    {
        options.Filters.Add<CustomExceptionFilter>();
        options.Filters.Add<ValidationFilter>();
    }
);
services.AddFluentValidationAutoValidation(options =>
{
    // Validate child properties and root collection elements
    options.ImplicitlyValidateChildProperties = true;
    options.ImplicitlyValidateRootCollectionElements = true;

    // Automatic registration of validators in assembly
});

services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.OperationFilter<AddRequiredHeaderParametersFilter>();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API", Version = "v1"
    });
});

services.AddInfrastructureServices(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();