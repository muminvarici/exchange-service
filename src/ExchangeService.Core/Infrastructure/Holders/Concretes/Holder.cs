using ExchangeService.Core.Infrastructure.Exceptions;
using ExchangeService.Core.Infrastructure.Holders.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ExchangeService.Core.Infrastructure.Holders.Concretes;

public class Holder : IHolder
{
    private bool _initialized;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public bool CheckUser(int userId, bool throwException = true)
    {
        if (!_initialized) InitializeData();
        if (userId == UserId && userId > 0) return true;
        if (throwException) throw new ServiceException(ServiceExceptionType.Unauthorized);
        return false;
    }

    public bool CheckUser(bool throwException = true)
    {
        if (!_initialized) InitializeData();
        if (UserId > 0) return true;
        if (throwException) throw new ServiceException(ServiceExceptionType.Unauthorized);
        return false;
    }

    public virtual string CustomRequestId { get; protected set; }
    public virtual string Ip { get; protected set; }
    public virtual string Language { get; set; }
    public virtual string Country { get; set; }
    public virtual string ApplicationId { get; set; }
    public virtual int UserId { get; set; }
    public virtual string RequestId { get; protected set; }
    public virtual string UserName { get; protected set; }

    public Holder(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public virtual void InitializeData()
    {
        if (_initialized)
            return;
        if (_httpContextAccessor.HttpContext == null)
            return;
        _initialized = true;

        CustomRequestId ??= _httpContextAccessor.HttpContext.Request.Headers["CustomRequestId"];
        Language ??= _httpContextAccessor.HttpContext.Request.Headers["Language"];
        Country ??= _httpContextAccessor.HttpContext.Request.Headers["Country"];
        Ip ??= _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(); //todo ip adresinin forward edildiğinde nasıl geldiğine bak
        ApplicationId ??= _httpContextAccessor.HttpContext.Request.Headers["ApplicationId"];
        RequestId ??= _httpContextAccessor.HttpContext.Request.Headers["x-request-id"];
        // UserName ??= _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Name)?.Value;
        UserName = _httpContextAccessor.HttpContext.Request.Headers["UserName"];
        if (UserId > 0) return;
        // var header = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
        var header = _httpContextAccessor.HttpContext.Request.Headers["UserId"];
        if (int.TryParse(header, out var userId))
            UserId = userId;
        UserName ??= UserId.ToString();
    }
}