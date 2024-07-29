
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class UserRepository(BaseDbContext baseDbContext) : BaseRepository<User>(baseDbContext), IUserRepository
{

    public async Task<IEnumerable<User>> GetAll()
    {
        var users = await Get()
            .Include(u => u.Permissions)
            .ToListAsync();

        return users;

    }

    public async Task<User?> FindByUsername(string username)
    {
        return await GetByCondition(user => user.Username.Equals(username))
            
            .FirstOrDefaultAsync();
    }

    public async Task<User?> FindDetailedById(int id)
    {
        return await GetByCondition(user => user.Id.Equals(id))
            .Include(u => u.Permissions)
            .Include(u => u.RefreshToken)
            .SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> FindById(int id)
    {
        return await GetByCondition(user => user.Id.Equals(id))
            .FirstOrDefaultAsync();
    }

    public async Task<User?> FindByRefreshToken(string refreshToken)
    {
        return await GetByCondition(user => user.RefreshToken.Token.Equals(refreshToken))
            .Include(u => u.RefreshToken)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetByPermission(Permission permission)
    {
        return await GetByCondition(user => user.Permissions.Contains(permission)).ToListAsync();
    }

    public void AddPermissionToUser(User user, Permission permission)
    {
        user.Permissions.Add(permission);
    }

    public void RemovePermissionFromUser(User user, Permission permission)
    {
        user.Permissions.Remove(permission);
    }

    public void CreateUser(User user)
    {
        Create(user);
    }

    public void UpdateUser(User user)
    {
        Update(user);
    }

    public void DeleteUser(User user)
    {
        Delete(user);
    }


}