namespace ExchangeService.Api.Filters
{
    public abstract class ResponseBase
    {
        public bool IsSuccess { get; protected set; }
    }

    public class SuccessResponse<T> : ResponseBase
    {
        public SuccessResponse()
        {
            IsSuccess = true;
        }

        public T Data { get; set; }
    }

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
}