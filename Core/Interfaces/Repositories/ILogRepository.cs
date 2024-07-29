

namespace Core.Interfaces.Repositories;
public interface ILogRepository : IBaseRepository<Log>
{
    Task<IEnumerable<Log>> GetAll();
    Task<Log> FindById(int id);
    Task<IEnumerable<Log>> GetByUserId(int user);
    void AddLog(Log log);
    void UpdateLog(Log log);
    void DeleteLog(Log log);
}