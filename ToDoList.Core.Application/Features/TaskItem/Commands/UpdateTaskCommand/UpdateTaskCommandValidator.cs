using FluentValidation;

namespace ToDoList.Core.Application.Features.TaskItem.Commands.UpdateTaskCommand
{
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator() {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Task ID is required.")
                .NotNull().WithMessage("Task ID cannot be null.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");

            RuleFor(x => x.AditionalData)
                .MaximumLength(1000).WithMessage("Additional data cannot exceed 1000 characters.");

            RuleFor(x => x.StatusTask)
                .IsInEnum().WithMessage("Invalid status task value.");

            RuleFor(x => x.TaskType)
                .IsInEnum().WithMessage("Invalid task type value.");
        }
    }
}
