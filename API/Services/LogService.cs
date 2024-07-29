

namespace API.Services;
public class LogService(IRepositoryWrapper repositoryWrapper) : ILogService
{
    readonly IRepositoryWrapper _repositoryWrapper = repositoryWrapper;

    public async Task AddLog(Log log)
    {
        _repositoryWrapper.Logs.AddLog(log);
        await _repositoryWrapper.Save();
    }

    public async Task<IEnumerable<Log>> GetLogs()
    {
        return await _repositoryWrapper.Logs.GetAll();
    }

    public async Task<IEnumerable<Log>> GetLogsByUser(int user)
    {
        return await _repositoryWrapper.Logs.GetByUserId(user);
    }
}