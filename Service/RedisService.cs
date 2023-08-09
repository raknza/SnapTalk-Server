namespace android_backend.Service;

using System;
using StackExchange.Redis;

public class RedisService
{
    private readonly ConnectionMultiplexer _redisConnection;
    private readonly IConfiguration configuration;

    public RedisService(IConfiguration configuration)
    {
        this.configuration = configuration;
        _redisConnection = ConnectionMultiplexer.Connect(configuration.GetValue<String>("RedisSettings:ConnectionStrings"));
    }

    public IDatabase GetDatabase(int db = -1)
    {
        return _redisConnection.GetDatabase(db);
    }

    public bool SetString(string key, string value, TimeSpan? expiry = null)
    {
        var db = GetDatabase();
        return db.StringSet(key, value, expiry);
    }

    public string GetString(string key)
    {
        var db = GetDatabase();
        return db.StringGet(key);
    }

    public void Dispose()
    {
        _redisConnection.Dispose();
    }       
}
