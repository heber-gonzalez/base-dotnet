// Purpose: Interface for the Auth Service. It contains methods for user authentication, user creation, token generation, and user management.

namespace Core.Interfaces.Auth;

public interface ITokenService
{
    Task<TokensDto> GetTokens(User user);
    Task<User> FindUserByRefreshToken(string token);
    Task<User> VerifyRefreshToken(string refreshToken);
    

}