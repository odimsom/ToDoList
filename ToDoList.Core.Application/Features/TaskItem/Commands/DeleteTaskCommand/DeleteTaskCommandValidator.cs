using FluentValidation;

namespace ToDoList.Core.Application.Features.TaskItem.Commands.DeleteTaskCommand
{
    public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
    {
        public DeleteTaskCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull()
                .WithMessage("The Id field is required.");
            RuleFor(x => x.Id)
                .Must(id => id != Guid.Empty)
                .WithMessage("The Id field must not be an empty GUID.");
            RuleFor(x => x.Id)
                .Must(id => id != default(Guid))
                .WithMessage("The Id field must not be the default GUID value.");
            RuleFor(x => x.Id)
                .Must(id => id != Guid.NewGuid())
                .WithMessage("The Id field must not be a new GUID value.");
            RuleFor(x => x.Id)
                .Must(id => id != Guid.Empty && id != default(Guid) && id != Guid.NewGuid())
                .WithMessage("The Id field must not be an empty, default, or new GUID value.");
            RuleFor(x => x.Id)
                .Must(id => id != Guid.Empty && id != default(Guid) && id != Guid.NewGuid() && id != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                .WithMessage("The Id field must not be an empty, default, new GUID value, or the zero GUID value.");
        }
    }   
}
