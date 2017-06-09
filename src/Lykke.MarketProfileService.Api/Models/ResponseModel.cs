namespace Lykke.MarketProfileService.Api.Models
{
    public class ResponseModel<TResult> : ResponseModel
    {
        public TResult Result { get; set; }
    }

    public class ResponseModel
    {
        public ErrorModel Error { get; set; }

        public static ResponseModel<TResult> CreateResult<TResult>(TResult result)
        {
            return new ResponseModel<TResult>
            {
                Result = result
            };
        }

        public static ResponseModel<TResult> CreateError<TResult>(ErrorModel error)
        {
            return new ResponseModel<TResult>
            {
                Error = error
            };
        }

        public static ResponseModel<TResult> CreateError<TResult>(ErrorCode code)
        {
            return CreateError<TResult>(code, null);
        }

        public static ResponseModel<TResult> CreateError<TResult>(ErrorCode code, string message)
        {
            return CreateError<TResult>(new ErrorModel
            {
                Code = code,
                Message = message
            });
        }

        public static ResponseModel CreateError(ErrorModel error)
        {
            return new ResponseModel
            {
                Error = error
            };
        }

        public static ResponseModel CreateError(ErrorCode code)
        {
            return CreateError(code, null);
        }

        public static ResponseModel CreateError(ErrorCode code, string message)
        {
            return CreateError(new ErrorModel
            {
                Code = code,
                Message = message
            });
        }
    }
}