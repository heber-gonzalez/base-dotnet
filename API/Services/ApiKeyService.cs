namespace API.Services;

public class ApiKeyService(IRepositoryWrapper repositoryWrapper) : IApiKeyService
{
    readonly IRepositoryWrapper _repositoryWrapper = repositoryWrapper;

    public async Task<ApiKey?> GetApiKeyById(int id)
    {
        return await _repositoryWrapper.ApiKey.FindById(id);
    }

    public async Task<ApiKey?> VerifyApiKey(string hashedKey)
    {
        return await _repositoryWrapper.ApiKey.FindByHashedKey(hashedKey);
    }



    // data
    public async Task SaveChanges()
    {
        await _repositoryWrapper.Save();
    }

    public async Task<ApiKey> CreateApiKey(ApiKey key)
    {
        _repositoryWrapper.ApiKey.CreateKey(key);
        await _repositoryWrapper.Save();
        return key;
    }

    public async Task UpdateApiKey(ApiKey key)
    {
        _repositoryWrapper.ApiKey.UpdateKey(key);
        await _repositoryWrapper.Save();
    }

    public async Task DeleteApiKey(ApiKey key)
    {
        _repositoryWrapper.ApiKey.DeleteKey(key);
        await _repositoryWrapper.Save();
    }



    
}