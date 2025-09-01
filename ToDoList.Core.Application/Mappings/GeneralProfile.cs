using ToDoList.Core.Application.Features.TaskItem.Commands.CreateTaskCommand;
using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Application.DTOs.Auth;
using ToDoList.Core.Application.Features.Auth.Commands;
using AutoMapper;

namespace ToDoList.Core.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            // TaskItem mappings
            CreateMap<TaskItem, CreateTaskCommand>();

            // Auth mappings
            CreateMap<User, UserDto>();
            CreateMap<RegisterRequestDto, RegisterCommand>();
            CreateMap<LoginRequestDto, LoginCommand>();
            CreateMap<RefreshTokenRequestDto, RefreshTokenCommand>();
        }
    }
}
