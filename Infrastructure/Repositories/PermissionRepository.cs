
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(BaseDbContext kazaDbContext) : base(kazaDbContext)
    {

    }
    public async Task<IEnumerable<Permission>> GetAll()
    {
        return await Get().ToListAsync();
    }

    public async Task<Permission> FindById(int id)
    {
        return await GetByCondition(permission => permission.Id.Equals(id)).FirstOrDefaultAsync();
    }

    public async Task<Permission> FindByName(string name)
    {
        return await GetByCondition(permission => permission.Nombre.Equals(name)).FirstOrDefaultAsync();
    }

    public async Task<List<Permission>> GetByUser(User user)
    {
        return await GetByCondition(permission => permission.Users.Contains(user)).ToListAsync();
    }

    public void CreatePermission(Permission permission)
    {
        Create(permission);
    }

    public void UpdatePermission(Permission permission)
    {
        Update(permission);
    }

    public void DeletePermission(Permission permission)
    {
        Delete(permission);
    }
}