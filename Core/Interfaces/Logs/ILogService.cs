namespace Core.Interfaces.Logs;

public interface ILogService
{
    Task AddLog(Log log);
    Task<IEnumerable<Log>> GetLogs();
    Task<IEnumerable<Log>> GetLogsByUser(int user);
}
