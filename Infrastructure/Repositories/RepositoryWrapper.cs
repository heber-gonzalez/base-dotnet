namespace Infrastructure.Repositories;
public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly BaseDbContext _baseDbContext;
    private IUserRepository _user;
    private IRefreshTokenRepository _refreshToken;
    private ILogRepository _logs;
    private IPermissionRepository _permission;
    private IApiKeyRepository _apiKey;

    public RepositoryWrapper(BaseDbContext baseDbContext)
    {
        _baseDbContext = baseDbContext;
    }

    public IUserRepository User
    {
        get
        {
            _user ??= new UserRepository(_baseDbContext);

            return _user;
        }
    }

    public IRefreshTokenRepository RefreshToken
    {
        get
        {
            _refreshToken ??= new RefreshTokenRepository(_baseDbContext);

            return _refreshToken;
        }
    }

    public ILogRepository Logs
    {
        get
        {
            _logs ??= new LogRepository(_baseDbContext);

            return _logs;
        }
    }

    public IPermissionRepository Permission
    {
        get
        {
            _permission ??= new PermissionRepository(_baseDbContext);

            return _permission;
        }
    }

    public IApiKeyRepository ApiKey
    {
        get
        {
            _apiKey ??= new ApiKeyRepository(_baseDbContext);

            return _apiKey;
        }
    }


    public async Task Save()
    {
        await _baseDbContext.SaveChangesAsync();
    }
}