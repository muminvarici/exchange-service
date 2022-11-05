namespace ExchangeService.Api.Filters
{
    public abstract class ResponseBase
    {
        public bool IsSuccess { get; protected set; }
    }
}