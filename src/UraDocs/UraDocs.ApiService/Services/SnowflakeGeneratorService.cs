using Simple.SnowFlake;

namespace UraDocs.ApiService.Services;

public class SnowflakeGeneratorService
{
    private readonly Worker _worker;

    public SnowflakeGeneratorService()
    {
        _worker = new Worker();
    }

    public long GetId()
    {
        return _worker.NextId();
    }
}
