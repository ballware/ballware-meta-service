namespace Ballware.Meta.Data.Ef.Configuration;

public sealed class StorageOptions
{
    public bool AutoMigrations { get; set; } = false;
    public string? SeedPath { get; set; }
    public bool AutoSeedAdminTenant { get; set; } = false;
    public bool EnableCaching { get; set; } = false;
}