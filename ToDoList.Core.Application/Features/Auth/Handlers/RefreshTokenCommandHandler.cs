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
    /// Handler for refresh token command
    /// </summary>
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResponseService<RefreshTokenResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseService<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing refresh token request");

                // Find refresh token
                var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
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

                    // Remove expired token
                    await _refreshTokenRepository.DeleteAsync(refreshToken, cancellationToken);

                    return new ResponseService<RefreshTokenResponseDto>("Refresh token is expired");
                }

                // Get user
                var userResult = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
                if (!userResult.IsSuccess || userResult.Data == null)
                {
                    _logger.LogWarning("User not found for refresh token: {UserId}", refreshToken.UserId);
                    return new ResponseService<RefreshTokenResponseDto>("User not found");
                }

                var user = userResult.Data;

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("User account is inactive for user {UserId}", user.Id);
                    return new ResponseService<RefreshTokenResponseDto>("Account is inactive");
                }

                // Generate new tokens
                var newAccessToken = _jwtService.GenerateAccessToken(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Update refresh token in database
                refreshToken.Token = newRefreshToken;
                refreshToken.ExpirationDate = DateTime.UtcNow.AddDays(7); // Extend for 7 more days
                await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

                var response = new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // Access token expires in 1 hour
                    TokenType = "Bearer"
                };

                _logger.LogInformation("Refresh token successful for user {UserId}", user.Id);
                return new ResponseService<RefreshTokenResponseDto>(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refresh token request");
                return new ResponseService<RefreshTokenResponseDto>("An error occurred during token refresh");
            }
        }
    }
}
