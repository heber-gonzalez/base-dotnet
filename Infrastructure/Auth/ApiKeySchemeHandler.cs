using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;


namespace Infrastructure.Auth;

public class ApiKeySchemeHandler(IRepositoryWrapper repositoryWrapper, IApiKeyService apiKeyService, IOptionsMonitor<ApiKeySchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : AuthenticationHandler<ApiKeySchemeOptions>(options, logger, encoder, clock)
{
    private readonly IRepositoryWrapper _repositoryWrapper = repositoryWrapper;
    private readonly IApiKeyService _apiKeyService = apiKeyService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out Microsoft.Extensions.Primitives.StringValues value))
        {
            return AuthenticateResult.Fail("No API Key provided");
        }

        if (!Guid.TryParse(value, out Guid apiKeyGuid))
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        using var sha256 = SHA256.Create();
        var hashedIncomingKey = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(value)));

        var apiKey = await GetApiKey(hashedIncomingKey);

        if (apiKey is null)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }
     
        var claims = CreateClaims(apiKey);
        var identity = new ClaimsIdentity(claims, nameof(ApiKeySchemeHandler));
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    protected virtual async Task<ApiKey?> GetApiKey(string hashedApiKey)
    {
        try
        {
            return await _apiKeyService.VerifyApiKey(hashedApiKey);
        }
        
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while retrieving API Key");
            throw;
        }
    }
    

    protected virtual Claim[] CreateClaims(ApiKey apiKey)
    {
        return
        [
            new(ClaimTypes.Authentication, "ApiKeyScheme"),
            new(ClaimTypes.Name, apiKey.Name),
            new(ClaimTypes.NameIdentifier, $"{apiKey.ApiKeyId}"),
            new(ClaimTypes.Role, apiKey.Name)
        ];
    }
}