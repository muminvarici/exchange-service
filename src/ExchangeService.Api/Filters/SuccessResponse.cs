namespace ExchangeService.Api.Filters;

public class SuccessResponse<T> : ResponseBase
{
    public SuccessResponse()
    {
        IsSuccess = true;
    }

    public T Data { get; set; }
}