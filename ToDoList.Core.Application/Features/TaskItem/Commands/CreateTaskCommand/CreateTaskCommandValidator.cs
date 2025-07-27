using FluentValidation;

namespace ToDoList.Core.Application.Features.TaskItem.Commands.CreateTaskCommand
{
    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .NotNull()
                .MaximumLength(250);

            RuleFor(x => x.DueDate)
                .NotEmpty()
                .NotNull()
                .Must(date => date > DateTime.UtcNow)   
                .WithMessage("The expiration date must be in the future.");

            RuleFor(x => x.TaskType)
                .IsInEnum()
                .WithMessage("The state must be a valid enumeration value.");
        }
    }
}
