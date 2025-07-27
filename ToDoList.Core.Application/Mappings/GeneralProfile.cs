using ToDoList.Core.Application.Features.TaskItem.Commands.CreateTaskCommand;
using ToDoList.Core.Domain.Entities;
using AutoMapper;

namespace ToDoList.Core.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<TaskItem, CreateTaskCommand>();
        }
    }
}
