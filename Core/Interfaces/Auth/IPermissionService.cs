

namespace Core.Interfaces.Auth;

public interface IPermissionService
{
    Task SaveChanges();
    Task<IEnumerable<Permission>> GetPermissions();
    Task<Permission> GetPermissionById(int id);
    Task<Permission> GetPermissionByName(string name);
    Task<List<Permission>> GetPermissionsByUser(User user);
}