// Purpose: Interface for the Auth Service. It contains methods for user authentication, user creation, token generation, and user management.


namespace Core.Interfaces.Users;

public interface IUserService
{
    // users
    Task<User?> FindDetailedById(int id);
    Task<User?> FindUserById(int id);
    Task<User?> FindUserByRefreshToken(string token);
    Task<IEnumerable<User?>> GetUsersByPermission(Permission permission);

    Task<User> VerifyUser(string username, string password);
    Task<User> EditUser(EditUserDto registerDto);
    Task<User> RestorePassword(int userID, string newPassword);
    Task<List<UserDto>> GetUsers();
    Task<UserDto> FindUserDtoById(int id);

    void Create(User user);
    Task UpdateUser(User user);  

    User CreateUser(RegisterDto registerDto);
    Task AddPermissionsToUser(User user, List<int> permissions);

    Task SaveChanges();

}