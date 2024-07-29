

namespace Core.Interfaces.Repositories;

public interface IApiKeyRepository : IBaseRepository<ApiKey>
{
    Task<ApiKey?> FindById(int id);
    Task<ApiKey?> FindByHashedKey(string hashedKey);
    void CreateKey(ApiKey key);
    void UpdateKey(ApiKey key);
    void DeleteKey(ApiKey key);
}

