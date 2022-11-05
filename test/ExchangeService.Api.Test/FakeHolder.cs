using ExchangeService.Core.Infrastructure.Holders.Concretes;
using Microsoft.AspNetCore.Http;

namespace ExchangeService.Api.Test;

public class FakeHolder : Holder
{
    public static int StaticUser = 1529;

    public FakeHolder(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
    }
    public override void InitializeData()
    {
        UserId = StaticUser;
    }
}