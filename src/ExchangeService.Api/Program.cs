using System.Reflection;
using ExchangeService.Api.Filters;
using ExchangeService.Core.Infrastructure.Filters;
using ExchangeService.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.AddRouting(w => w.LowercaseUrls = true);
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
        Title = "API",
        Version = "v1"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

services.AddInfrastructureServices(configuration);

var app = builder.Build();

await app.Services.CreateScope().ServiceProvider.GetRequiredService<DbContext>().Database.EnsureCreatedAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exchange API V1");
        c.RoutePrefix = "";
        c.DocExpansion(DocExpansion.None);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();