


namespace API.Services;

public class PermissionService(IRepositoryWrapper repositoryWrapper) : IPermissionService
{
    readonly IRepositoryWrapper _repositoryWrapper = repositoryWrapper;

    public async Task SaveChanges()
    {
        await _repositoryWrapper.Save();
    }

    public async Task<IEnumerable<Permission>> GetPermissions()
    {
        return await _repositoryWrapper.Permission.GetAll();
    }

    public async Task<Permission> GetPermissionById(int id)
    {
        return await _repositoryWrapper.Permission.FindById(id);
    }

    public async Task<Permission> GetPermissionByName(string name)
    {
        return await _repositoryWrapper.Permission.FindByName(name);
    }

    public async Task<List<Permission>> GetPermissionsByUser(User user)
    {
        return await _repositoryWrapper.Permission.GetByUser(user);
    }

    private async Task CreatePermission(string permission)
    {
        _repositoryWrapper.Permission.CreatePermission(new Permission { Nombre = permission });
        await _repositoryWrapper.Save();
    }
    private async Task UpdatePermission(Permission permission)
    {
        _repositoryWrapper.Permission.UpdatePermission(permission);
        await _repositoryWrapper.Save();
    }

    private async Task DeletePermission(Permission permission)
    {
        _repositoryWrapper.Permission.DeletePermission(permission);
        await _repositoryWrapper.Save();
    }

    

}