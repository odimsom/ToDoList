using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Core.Application.Features.Auth.Commands;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.RepositoriesInterfaces;

namespace ToDoList.Core.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handler for logout command
    /// </summary>
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ResponseService<bool>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<LogoutCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<ResponseService<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing logout request for user {UserId}", request.UserId);

                // Revoke all user's refresh tokens
                await _refreshTokenRepository.RevokeAllUserTokensAsync(request.UserId, "User logout");

                _logger.LogInformation("Logout successful for user {UserId}", request.UserId);
                return new ResponseService<bool>(true, "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing logout request for user {UserId}", request.UserId);
                return new ResponseService<bool>("An error occurred during logout");
            }
        }
    }
}
