namespace ExchangeService.Api.Filters;

public class ExceptionResponse : ResponseBase
{
    public object[] Args { get; set; }
    public string Code { get; set; }

    public ExceptionResponse()
    {
        IsSuccess = false;
    }

    public string Message { get; set; }

    public Dictionary<object, object> Errors { get; set; }
}