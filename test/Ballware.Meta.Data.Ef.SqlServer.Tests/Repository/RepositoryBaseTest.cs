using System.Diagnostics;
using Ballware.Meta.Caching;
using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Ef.SqlServer;
using Ballware.Meta.Data.Ef.Tests.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public sealed class NUnitLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new NUnitLogger(categoryName);

    public void Dispose() { }

    private class NUnitLogger : ILogger
    {
        private readonly string _categoryName;

        public NUnitLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            if (exception != null)
            {
                TestContext.Progress.WriteLine($"[{logLevel}] {_categoryName}: {formatter(state, exception)}");    
            }
        }
    }
}

public class RepositoryBaseTest : DatabaseBackedBaseTest
{
    protected Guid TenantId { get; private set; }

    protected WebApplication Application { get; private set; }
    
    protected Mock<ITenantAwareEntityCache> TenantAwareEntityCacheMock { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task SetupApplication()
    {
        await base.MssqlSetUp();
        
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public async Task TearDownApplication()
    {
        await Application.DisposeAsync();
        await base.MssqlTearDown();
    }

    [SetUp]
    public async Task SetupTenantId()
    {
        base.SetupApplicationBuilder();
        
        TenantAwareEntityCacheMock = new Mock<ITenantAwareEntityCache>();
        
        var storageOptions = PreparedBuilder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = MasterConnectionString;

        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });

        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");

        PreparedBuilder.Services.AddLogging(config =>
        {
            config.AddProvider(new NUnitLoggerProvider());
        });
            
        PreparedBuilder.Services.AddSingleton<ITenantAwareEntityCache>(TenantAwareEntityCacheMock.Object);
        PreparedBuilder.Services.AddBallwareMetaStorageForSqlServer(storageOptions, connectionString);
        PreparedBuilder.Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
        });

        Application = PreparedBuilder.Build();
        
        TenantId = Guid.NewGuid();

        using var scope = Application.Services.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<MetaDbContext>();

        await dbContext.Database.MigrateAsync();
        
        var seeder = scope.ServiceProvider.GetRequiredService<IMetadataSeeder>();

        await seeder.SeedCustomerTenantAsync(TenantId, $"Customer_{TenantId.ToString()}");
    }
    
    [TearDown]
    public async Task TearDownTenantId()
    {
        await Application.DisposeAsync();
    }
}