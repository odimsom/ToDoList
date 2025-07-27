using FluentValidation.Results;

namespace ToDoList.Core.Application.Exceptions
{
    public class ValidationExceptions : Exception
    {
        public List<string> Errors { get; }
        public ValidationExceptions() : base("One or more validation errors have occurred")
        {
            Errors = new List<string>();
        }
        public ValidationExceptions(IEnumerable<ValidationFailure> failures) : this()
        {
            foreach (var failure in failures)
            {
                Errors.Add(failure.ErrorMessage);
            }
        }
    }
}
