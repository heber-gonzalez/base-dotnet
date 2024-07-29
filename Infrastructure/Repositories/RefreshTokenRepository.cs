using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly BaseDbContext _baseDbContext;

        public RefreshTokenRepository(BaseDbContext baseDbContext) : base(baseDbContext)
        {
            _baseDbContext = baseDbContext;
        }

        public async Task<RefreshToken> FindByToken(string token)
        {
            return await GetByCondition(refreshToken => refreshToken.Token.Equals(token)).FirstOrDefaultAsync();
        }

        public async Task<RefreshToken> FindByUserId(int userId)
        {
            return await GetByCondition(refreshToken => refreshToken.UserId.Equals(userId)).FirstOrDefaultAsync();
        }

        public void AddToken(RefreshToken refreshToken)
        {
            Create(refreshToken);
        }

        public void UpdateToken(RefreshToken refreshToken)
        {
            Update(refreshToken);
        }

        public void DeleteToken(RefreshToken refreshToken)
        {
            Delete(refreshToken);
        }
    }