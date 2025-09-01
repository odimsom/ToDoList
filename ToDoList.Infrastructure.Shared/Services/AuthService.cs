using AutoMapper;
using Microsoft.Extensions.Logging;
using ToDoList.Core.Application.DTOs.Auth;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.RepositoriesInterfaces;

namespace ToDoList.Infrastructure.Shared.Services
{
    /// <summary>
    /// Service implementation for authentication operations
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHashService passwordHashService,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHashService = passwordHashService;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseService<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Processing login request for user: {UsernameOrEmail}", loginRequest.UsernameOrEmail);

                // Find user by username or email
                var user = await FindUserByUsernameOrEmailAsync(loginRequest.UsernameOrEmail);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found for {UsernameOrEmail}", loginRequest.UsernameOrEmail);
                    return new ResponseService<LoginResponseDto>("Invalid credentials");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed: User account is inactive for {UsernameOrEmail}", loginRequest.UsernameOrEmail);
                    return new ResponseService<LoginResponseDto>("Account is inactive");
                }

                // Verify password
                if (!_passwordHashService.VerifyPassword(loginRequest.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                    return new ResponseService<LoginResponseDto>("Invalid credentials");
                }

                // Update last login date
                user.LastLoginDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user, CancellationToken.None);

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Create refresh token entity
                var refreshTokenEntity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = refreshToken,
                    UserId = user.Id,
                    ExpirationDate = DateTime.UtcNow.AddDays(loginRequest.RememberMe ? 30 : 7),
                    IsRevoked = false
                };

                await _refreshTokenRepository.AddAsync(refreshTokenEntity, CancellationToken.None);

                // Map user to DTO
                var userDto = _mapper.Map<UserDto>(user);

                var response = new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    User = userDto,
                    TokenType = "Bearer"
                };

                _logger.LogInformation("Login successful for user {UserId}", user.Id);
                return new ResponseService<LoginResponseDto>(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing login request for {UsernameOrEmail}", loginRequest.UsernameOrEmail);
                return new ResponseService<LoginResponseDto>("An error occurred during login");
            }
        }

        public async Task<ResponseService<RegisterResponseDto>> RegisterAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                _logger.LogInformation("Processing registration request for username: {Username}", registerRequest.Username);

                // Check if username already exists
                if (await _userRepository.UsernameExistsAsync(registerRequest.Username))
                {
                    _logger.LogWarning("Registration failed: Username {Username} already exists", registerRequest.Username);
                    return new ResponseService<RegisterResponseDto>("Username already exists");
                }

                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(registerRequest.Email))
                {
                    _logger.LogWarning("Registration failed: Email {Email} already exists", registerRequest.Email);
                    return new ResponseService<RegisterResponseDto>("Email already exists");
                }

                // Hash password
                var passwordHash = _passwordHashService.HashPassword(registerRequest.Password);

                // Create new user entity
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = registerRequest.Username,
                    Email = registerRequest.Email.ToLowerInvariant(),
                    PasswordHash = passwordHash,
                    FirstName = registerRequest.FirstName?.Trim(),
                    LastName = registerRequest.LastName?.Trim(),
                    Role = registerRequest.Role,
                    IsActive = true
                };

                // Save user to database
                var saveResult = await _userRepository.AddAsync(user, CancellationToken.None);

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
                _logger.LogError(ex, "Error processing registration request for username: {Username}", registerRequest.Username);
                return new ResponseService<RegisterResponseDto>("An error occurred during registration");
            }
        }

        public async Task<ResponseService<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto refreshRequest, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing refresh token request");

                // Find refresh token
                var refreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshRequest.RefreshToken);
                if (refreshToken == null)
                {
                    _logger.LogWarning("Refresh token not found");
                    return new ResponseService<RefreshTokenResponseDto>("Invalid refresh token");
                }

                // Check if token is revoked
                if (refreshToken.IsRevoked)
                {
                    _logger.LogWarning("Refresh token is revoked for user {UserId}", refreshToken.UserId);
                    return new ResponseService<RefreshTokenResponseDto>("Refresh token is revoked");
                }

                // Check if token is expired
                if (refreshToken.ExpirationDate <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh token is expired for user {UserId}", refreshToken.UserId);
                    await _refreshTokenRepository.DeleteAsync(refreshToken, cancellationToken);
                    return new ResponseService<RefreshTokenResponseDto>("Refresh token is expired");
                }

                // Get user
                var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found for refresh token: {UserId}", refreshToken.UserId);
                    return new ResponseService<RefreshTokenResponseDto>("User not found");
                }

                // Check if user is active
                if (!user.IsSuccess)
                {
                    _logger.LogWarning("User account is inactive for user {UserId}", user.Data!.Id);
                    return new ResponseService<RefreshTokenResponseDto>("Account is inactive");
                }

                // Generate new tokens
                var newAccessToken = _jwtService.GenerateAccessToken(user.Data!);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Update refresh token in database
                refreshToken.Token = newRefreshToken;
                refreshToken.ExpirationDate = DateTime.UtcNow.AddDays(7);
                await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

                var response = new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    TokenType = "Bearer"
                };

                _logger.LogInformation("Refresh token successful for user {UserId}", user.Data!.Id);
                return new ResponseService<RefreshTokenResponseDto>(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refresh token request");
                return new ResponseService<RefreshTokenResponseDto>("An error occurred during token refresh");
            }
        }

        public async Task<ResponseService<bool>> RevokeTokenAsync(string token, Guid userId)
        {
            try
            {
                _logger.LogInformation("Processing revoke token request for user {UserId}", userId);

                var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
                if (refreshToken == null || refreshToken.UserId != userId)
                {
                    _logger.LogWarning("Refresh token not found or unauthorized for user {UserId}", userId);
                    return new ResponseService<bool>("Invalid refresh token");
                }

                refreshToken.IsRevoked = true;
                refreshToken.RevokedBy = userId.ToString();
                refreshToken.RevokedDate = DateTime.UtcNow;

                await _refreshTokenRepository.UpdateAsync(refreshToken, CancellationToken.None);

                _logger.LogInformation("Token revoked successfully for user {UserId}", userId);
                return new ResponseService<bool>(true, "Token revoked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing revoke token request for user {UserId}", userId);
                return new ResponseService<bool>("An error occurred during token revocation");
            }
        }

        public async Task<ResponseService<bool>> LogoutAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Processing logout request for user {UserId}", userId);

                await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, "User logout");

                _logger.LogInformation("Logout successful for user {UserId}", userId);
                return new ResponseService<bool>(true, "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing logout request for user {UserId}", userId);
                return new ResponseService<bool>("An error occurred during logout");
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

        public Task<ResponseService<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto refreshRequest)
        {
            throw new NotImplementedException();
        }
    }
}
