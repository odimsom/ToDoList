using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Core.Application.DTOs.Auth;
using ToDoList.Core.Application.Features.Auth.Commands;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.RepositoriesInterfaces;

namespace ToDoList.Core.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handler for user registration command
    /// </summary>
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ResponseService<RegisterResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            IMapper mapper,
            ILogger<RegisterCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseService<RegisterResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing registration request for username: {Username}", request.Username);

                // Check if username already exists
                if (await _userRepository.UsernameExistsAsync(request.Username))
                {
                    _logger.LogWarning("Registration failed: Username {Username} already exists", request.Username);
                    return new ResponseService<RegisterResponseDto>("Username already exists");
                }

                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("Registration failed: Email {Email} already exists", request.Email);
                    return new ResponseService<RegisterResponseDto>("Email already exists");
                }

                // Hash password
                var passwordHash = _passwordHashService.HashPassword(request.Password);

                // Create new user entity
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = request.Email.ToLowerInvariant(),
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName?.Trim(),
                    LastName = request.LastName?.Trim(),
                    Role = request.Role,
                    IsActive = true
                };

                // Save user to database
                await _userRepository.AddAsync(user, cancellationToken);

                // Map user to DTO
                var userDto = _mapper.Map<UserDto>(user);

                var response = new RegisterResponseDto
                {
                    User = userDto,
                    Message = "User registered successfully"
                };

                _logger.LogInformation("Registration successful for user {UserId}", user.Id);
                return new ResponseService<RegisterResponseDto>(response, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing registration request for username: {Username}", request.Username);
                return new ResponseService<RegisterResponseDto>("An error occurred during registration");
            }
        }
    }
}
