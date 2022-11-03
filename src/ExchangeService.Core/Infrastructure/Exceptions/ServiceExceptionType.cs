namespace ExchangeService.Core.Infrastructure.Exceptions;

public enum ServiceExceptionType
{
    None = 0,
    NotFound = 1, //404
    BadRequest = 2, //400
    Conflict = 3, //409
    Unauthorized = 4, //401
    Forbidden = 5 //403
}