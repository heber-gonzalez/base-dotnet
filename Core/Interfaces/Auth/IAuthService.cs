// Purpose: Interface for the Auth Service. It contains methods for user authentication, user creation, token generation, and user management.

namespace Core.Interfaces.Auth;

public interface IAuthService
{
    // USERS
    Task<User> Register(RegisterDto registerDto);
    Task<TokensDto> Login(LoginDto loginDto);
    Task<TokensDto> RefreshTokens(string refreshToken);
    Task<User> EditUser(EditUserDto registerDto);
    Task<User> RestorePassword(int userID, string newPassword);
    Task<List<UserDto>> GetUsers();
    Task<UserDto> FindUserDtoById(int id);
    Task UpdateUser(User user);  

    Task SaveUser();

    // PERMISSIONS
    Task<IEnumerable<Permission>> GetPermissions();

}