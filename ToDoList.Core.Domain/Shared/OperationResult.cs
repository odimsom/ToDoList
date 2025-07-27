namespace ToDoList.Core.Domain.Shared
{
    /// <summary>
    /// Represents the result of an operation, encapsulating the outcome, data, messages, and errors.
    /// </summary>
    /// <typeparam name="TData">The type of data returned by the operation.</typeparam>
    public class OperationResult<TData>
    {
        /// <summary>
        /// Gets or sets the data returned by the operation, if any.
        /// </summary>
        public TData? Data { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// Gets or sets an optional message describing the result of the operation.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Gets or sets an array of error messages if the operation failed.
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TData}"/> class.
        /// </summary>
        /// <param name="data">The data returned by the operation.</param>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="message">A message describing the result.</param>
        /// <param name="errors">An array of error messages, if any.</param>
        private OperationResult(TData? data, bool isSuccess, string? message, List<string>? errors)
        {
            Data = data;
            IsSuccess = isSuccess;
            Message = message;
            Errors = errors;
        }

        public OperationResult()
        {
        }

        /// <summary>
        /// Creates a successful <see cref="OperationResult{TData}"/> instance.
        /// </summary>
        /// <param name="data">The data returned by the operation.</param>
        /// <param name="message">An optional success message. Defaults to "Operation is Success".</param>
        /// <returns>An <see cref="OperationResult{TData}"/> representing a successful operation.</returns>
        public static OperationResult<TData> Success(TData data, string? message = "Operation is Success") => 
            new OperationResult<TData>(data, true, message, null);

        /// <summary>
        /// Creates a failed <see cref="OperationResult{TData}"/> instance.
        /// </summary>
        /// <param name="errors">An array of error messages describing the failure.</param>
        /// <param name="message">An optional failure message. Defaults to "Operation Failure".</param>
        /// <param name="data">Optional data returned by the operation. Defaults to the default value of <typeparamref name="TData"/>.</param>
        /// <returns>An <see cref="OperationResult{TData}"/> representing a failed operation.</returns>
        public static OperationResult<TData> Failure(List<string> errors, string? message = "Operation Failure", TData data = default!) => 
            new OperationResult<TData>(data ?? data, false, message, errors);
    }
}
