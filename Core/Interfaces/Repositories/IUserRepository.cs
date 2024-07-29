

namespace Core.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> FindByUsername(string username);
    Task<User?> FindDetailedById(int id);
    Task<User?> FindById(int id);
    Task<User?> FindByRefreshToken(string token);

    Task<IEnumerable<User>> GetByPermission(Permission permission);
    void AddPermissionToUser(User user, Permission permission);
    void RemovePermissionFromUser(User user, Permission permission);

    void CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
}