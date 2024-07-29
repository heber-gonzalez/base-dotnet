// Purpose: Interface for the ApiKey Service. It contains methods for creating, deleting, and verifying API keys.
namespace Core.Interfaces.Auth;

public interface IApiKeyService
{
    Task<ApiKey?> GetApiKeyById(int id);
    Task<ApiKey?> VerifyApiKey(string hashedKey);
    Task<ApiKey> CreateApiKey(ApiKey name);

    Task UpdateApiKey(ApiKey apiKey);
    Task DeleteApiKey(ApiKey id);
    Task SaveChanges();
}