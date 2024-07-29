// Purpose: Interface for RefreshTokenRepository. This file contains all the methods that RefreshTokenRepository must implement.

namespace Core.Interfaces.Repositories;
public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken> FindByToken(string token); //get a refresh token by token value
    Task<RefreshToken> FindByUserId(int userId); //get a refresh token by user id
    void AddToken(RefreshToken refreshToken); //add a new refresh token to the database
    void UpdateToken(RefreshToken refreshToken); //update an existing refresh token in the database
    void DeleteToken(RefreshToken refreshToken); //delete an existing refresh token from the database
}