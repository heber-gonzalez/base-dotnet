using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class LogRepository : BaseRepository<Log>, ILogRepository
{
    private readonly BaseDbContext _context;
    public LogRepository(BaseDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Log>> GetAll()
    {
        return await base.Get().ToListAsync();
    }

    public async Task<Log> FindById(int id)
    {
        return await GetByCondition(log => log.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Log>> GetByUserId(int user)
    {
        return await GetByCondition(log => log.UsuarioID == user).ToListAsync();
    }

    public void AddLog(Log log)
    {
        Create(log);
    }

    public void UpdateLog(Log log)
    {
        Update(log);
    }

    public void DeleteLog(Log log)
    {
        Delete(log);
    }
}