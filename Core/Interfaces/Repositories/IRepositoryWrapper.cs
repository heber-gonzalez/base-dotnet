// Purpose: Interface for the RepositoryWrapper class in the Core project.
namespace Core.Interfaces.Repositories;
public interface IRepositoryWrapper
{
    IUserRepository User { get; }
    IRefreshTokenRepository RefreshToken { get; }
    ILogRepository Logs { get; }
    IPermissionRepository Permission { get; }
    IApiKeyRepository ApiKey { get; }
    Task Save();
}