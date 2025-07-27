using ToDoList.Core.Domain.Shared;

namespace ToDoList.Core.Application.Wrapper
{
    public class ResponseService<TData> : OperationResult<TData>
    {
        public int StatusCode { get; set; }
        public OperationResult<TData> OperationResult { get; set; }
        private ResponseService(OperationResult<TData> operationResult, int statusCode)
        {
            StatusCode = statusCode;
            OperationResult = operationResult;
        }
        public ResponseService()
        {
        }
        public static ResponseService<TData> ResponseSuccess(TData? data, string message, int statusCode) => new ResponseService<TData>(OperationResult<TData>.Success(data!, message??default),statusCode);
        public static ResponseService<TData> ResponseFailure(int statusCode, List<string> errors, string? message, TData? data) => new ResponseService<TData>(OperationResult<TData>.Failure(errors,message??default, data??default!), statusCode);
    }
}
