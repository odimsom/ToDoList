using FluentValidation;
using MediatR;
using System.Reflection;
using ToDoList.Core.Application.Wrapper;

namespace ToDoList.Core.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var errors = failures.Select(f => f.ErrorMessage).ToList();

                    var responseType = typeof(TResponse);
                    var tDataType = responseType.IsGenericType
                        ? responseType.GetGenericArguments()[0]
                        : typeof(object);

                    var method = typeof(ResponseService<>)
                        .MakeGenericType(tDataType)
                        .GetMethod("ResponseFailure", BindingFlags.Public | BindingFlags.Static);

                    var response = method!.Invoke(null, new object?[] { 400, errors, "One or more validation errors have occurred", null });

                    return (TResponse)response!;
                }
            }
            return await next();
        }
    }
}
