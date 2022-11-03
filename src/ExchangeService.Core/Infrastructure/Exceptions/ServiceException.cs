namespace ExchangeService.Core.Infrastructure.Exceptions;

/// <summary>
/// Exceptions to manipulate business exceptions
/// </summary>
public class ServiceException : Exception
{
    public object[] Args { get; private set; }
    public string Code { get; private set; }
    public ServiceException(ServiceExceptionType type) : this(type, "error", null)
    {

    }
    public ServiceException(ServiceExceptionType type, string code):this(type,code,null)
    {
        
    }

    public ServiceException(ServiceExceptionType type, string code, string message, params object[] args) : base(message)
    {
        Type = type;
        Args = args;
        Code = code;
    }

    public ServiceExceptionType Type { get; }

    /// <summary>
    /// server MUST respond with 404 Not Found when processing a request to fetch a single resource that does not exist,
    /// except when the request warrants a 200 OK response with null as the primary data
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceException NotFoundException(string code, string message = "")
    {
        return new ServiceException(ServiceExceptionType.NotFound, code, message);
    }

    /// <summary>
    /// If the server does not support parameter as specified in the query parameter , MUST return 400 Bad Request.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static ServiceException BadRequestException(string code, string message = "", params object[] args)
    {
        return new ServiceException(ServiceExceptionType.BadRequest, code, message, args);
    }

    /// <summary>
    /// server MUST return ExistsException when processing a request to create a resource with a client-generated ID that already exists
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static ServiceException ExistsException(string code, string message = "", params object[] args)
    {
        return new ServiceException(ServiceExceptionType.Conflict, code, message, args);
    }

    /// <summary>
    /// server MUST either completely replace every member of the relationship,
    /// return an appropriate error response if some resources can not be found or accessed,
    /// or return a ForbiddenException if complete replacement is not allowed by the server.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>

    public static ServiceException ForbiddenException(string code, string message = "", params object[] args)
    {
        return new ServiceException(ServiceExceptionType.Forbidden, code, message, args);
    }
}