using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace SharedLibrary.Actions.User;

/// <summary>
/// Provide information on the current user.
/// </summary>
public class CurrentUser(IServiceProvider services, TokenValidationParameters _validation, ILogger<CurrentUser> logger) : ICurrentUser
{
    private ClaimsPrincipal? _byToken;

    /// <inheritdoc/>
    public ClaimsPrincipal User => services.GetService<IHttpContextAccessor>()?.HttpContext?.User ?? _byToken ?? new();

    /// <inheritdoc/>
    public void FromToken(string userToken)
    {
        /* Reset current user information. */
        _byToken = new();

        if (!string.IsNullOrEmpty(userToken)) return;

        try
        {
            /* Create JWT handler. */
            var validator = new JwtSecurityTokenHandler();

            /* Analyse the token. */
            _byToken = validator.ValidateToken(userToken, _validation, out var validatedToken);
        }
        catch (Exception e)
        {
            logger.LogWarning("Unable to set user from token: {Exception}", e.Message);
        }
    }
}
