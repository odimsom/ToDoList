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
    /// Handler for user login command
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ResponseService<LoginResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHashService passwordHashService,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHashService = passwordHashService;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseService<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing login request for user: {UsernameOrEmail}", request.UsernameOrEmail);

                // Find user by username or email
                var user = await FindUserByUsernameOrEmailAsync(request.UsernameOrEmail);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found for {UsernameOrEmail}", request.UsernameOrEmail);
                    return new ResponseService<LoginResponseDto>("Invalid credentials");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed: User account is inactive for {UsernameOrEmail}", request.UsernameOrEmail);
                    return new ResponseService<LoginResponseDto>("Account is inactive");
                }

                // Verify password
                if (!_passwordHashService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                    return new ResponseService<LoginResponseDto>("Invalid credentials");
                }

                // Update last login date
                user.LastLoginDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user, cancellationToken);

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Create refresh token entity
                var refreshTokenEntity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = refreshToken,
                    UserId = user.Id,
                    ExpirationDate = DateTime.UtcNow.AddDays(request.RememberMe ? 30 : 7), // 30 days if remember me, otherwise 7 days
                    IsRevoked = false
                };

                await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

                // Map user to DTO
                var userDto = _mapper.Map<UserDto>(user);

                var response = new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // Access token expires in 1 hour
                    User = userDto,
                    TokenType = "Bearer"
                };

                _logger.LogInformation("Login successful for user {UserId}", user.Id);
                return new ResponseService<LoginResponseDto>(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing login request for {UsernameOrEmail}", request.UsernameOrEmail);
                return new ResponseService<LoginResponseDto>("An error occurred during login");
            }
        }

        private async Task<User?> FindUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            // Try to find by username first
            var user = await _userRepository.GetByUsernameAsync(usernameOrEmail);

            // If not found, try by email
            if (user == null && usernameOrEmail.Contains("@"))
            {
                user = await _userRepository.GetByEmailAsync(usernameOrEmail);
            }

            return user;
        }
    }
}
