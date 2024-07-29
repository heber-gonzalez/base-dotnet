

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ApiKeyRepository(BaseDbContext context) : BaseRepository<ApiKey>(context), IApiKeyRepository
{
    private readonly BaseDbContext _context = context;

    public async Task<ApiKey?> FindById(int id)
    {
        return await GetByCondition(key => key.ApiKeyId == id).FirstOrDefaultAsync();
    }

    public async Task<ApiKey?> FindByHashedKey(string hashedKey)
    {
        return await GetByCondition(key => key.HashedKey == hashedKey).FirstOrDefaultAsync();
    }

    public void CreateKey(ApiKey key)
    {
        Create(key);
    }

    public void UpdateKey(ApiKey key)
    {
        Update(key);
    }

    public void DeleteKey(ApiKey key)
    {
        Delete(key);
    }

}
