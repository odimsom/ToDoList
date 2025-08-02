namespace ToDoList.Core.Application.Wrapper
{
    public class ResponseApi<TData>
    {
        public TData? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
    }
}
