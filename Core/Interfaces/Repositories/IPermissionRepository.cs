// Purpose: Interface for Permission Repository.

namespace Core.Interfaces.Repositories;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task<IEnumerable<Permission>> GetAll();
    Task<Permission> FindById(int id);
    Task<Permission> FindByName(string name);
    Task<List<Permission>> GetByUser(User user);
    
    void CreatePermission(Permission permission);
    void UpdatePermission(Permission permission);
    void DeletePermission(Permission permission);
}