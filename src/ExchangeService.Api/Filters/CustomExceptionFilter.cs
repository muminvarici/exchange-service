using System.Net;
using ExchangeService.Core.Infrastructure.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExchangeService.Api.Filters;

public class CustomExceptionFilter : ExceptionFilterAttribute
{
    readonly ILogger<CustomExceptionFilter> _logger;

    public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = (int)GetStatusCode(context.Exception);
        var exp = context.Exception switch
        {
            ValidationException ve => new ExceptionResponse
            {
                //Code = ve.HResult,
                Errors = ve.Errors.ToDictionary(w => (object)w.PropertyName, x => (object)x.ErrorMessage),
                Code = "validation.error",
                Message = "Validation Error"
            },
            ServiceException se => new ExceptionResponse
            {
                //Code = se.HResult,
                Message = se.Message,
                Args = se.Args,
                Code = se.Code
            },
            { } e => new ExceptionResponse
            {
                Code = "system.error",
                //Code = -1,
                Message = $"{e.Message}({e.GetType().Name})"
            }
        };
        _logger.LogError(context.Exception, context.Exception.Message);
        context.Result = new JsonResult(exp);
    }

    /*
        - 1xx: Informational - Request received, continuing process
        - 2xx: Success - The action was successfully received, understood, and accepted
        - 3xx: Redirection - Further action must be taken in order to complete the request
        - 4xx: Client Error - The request contains bad syntax or cannot be fulfilled
        - 5xx: Server Error - The server failed to fulfill an apparently valid request
    */

    private HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            ServiceException { Type: ServiceExceptionType.NotFound } => HttpStatusCode.NotFound,
            ServiceException { Type: ServiceExceptionType.BadRequest } => HttpStatusCode.BadRequest,
            ServiceException { Type: ServiceExceptionType.Conflict } => HttpStatusCode.Conflict,
            ServiceException { Type: ServiceExceptionType.Unauthorized } => HttpStatusCode.Unauthorized,
            ServiceException { Type: ServiceExceptionType.Forbidden } => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };
    }
}