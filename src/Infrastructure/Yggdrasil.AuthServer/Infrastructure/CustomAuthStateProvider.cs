using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Yggdrasil.AuthServer.Infrastructure
{
    public class CustomAuthStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CustomAuthStateProvider> _logger;

        public CustomAuthStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory,
            IOptions<IdentityOptions> optionsAccessor): base(loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = loggerFactory.CreateLogger<CustomAuthStateProvider>();
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState,
            CancellationToken cancellationToken)
        {
            // Obtener el usuario actual
            var user = authenticationState.User;
            if (!user.Identity.IsAuthenticated)
                return false;

            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            // Verificar si el usuario aún existe
            var userId = userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return false;

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                _logger.LogWarning("Usuario {UserId} no encontrado", userId);
                return false;
            }

            // Verificar si el usuario está bloqueado
            if (await userManager.IsLockedOutAsync(currentUser))
            {
                _logger.LogWarning("Usuario {UserId} está bloqueado", userId);
                return false;
            }

            return true;
        }
    }
}
