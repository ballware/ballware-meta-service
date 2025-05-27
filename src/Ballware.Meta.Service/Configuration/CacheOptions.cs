namespace Ballware.Meta.Service.Configuration;

public class CacheOptions
{
    public string RedisConfiguration { get; set; } = string.Empty;
    public string RedisInstanceName { get; set; } = "ballware.meta:";
    public int CacheExpirationHours { get; set; } = 1;
}