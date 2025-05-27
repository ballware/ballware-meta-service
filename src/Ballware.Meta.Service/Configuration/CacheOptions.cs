namespace Ballware.Meta.Service.Configuration;

public class CacheOptions : Ballware.Meta.Caching.Configuration.CacheOptions
{
    public string RedisConfiguration { get; set; } = string.Empty;
    public string RedisInstanceName { get; set; } = "ballware.meta:";
}