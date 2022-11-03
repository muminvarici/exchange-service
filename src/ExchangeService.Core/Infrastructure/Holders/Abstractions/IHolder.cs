namespace ExchangeService.Core.Infrastructure.Holders.Abstractions;

public interface IHolder
{
    bool CheckUser(int userId, bool throwException = true);
    string CustomRequestId { get; }

    string Ip { get; }

    string Language { get; set; }

    string Country { get; set; }

    string ApplicationId { get; set; }

    int UserId { get; set; }

    string RequestId { get; }
    string UserName { get; }

    void InitializeData();
}