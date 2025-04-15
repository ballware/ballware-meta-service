using System.Diagnostics;
using Ballware.Meta.Data.Ef.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public class NUnitLoggerProvider : ILoggerProvider
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

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                TestContext.Progress.WriteLine($"[{logLevel}] {_categoryName}: {formatter(state, exception)}");
            }
        }
    }
}

public class RepositoryBaseTest
{
    protected Guid TenantId { get; private set; }

    protected WebApplication Application { get; private set; }

    [OneTimeSetUp]
    public void SetupApplication()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.Sources.Clear();
        builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings_with_migrations.json"), optional: false);
        builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings_with_migrations.{builder.Environment.EnvironmentName}.json"), true, true);
        builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings_with_migrations.local.json"), true, true);
        builder.Configuration.AddEnvironmentVariables();

        var storageOptions = builder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = builder.Configuration.GetConnectionString("MetaConnection");

        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });

        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");

        builder.Services.AddLogging(config =>
        {
            config.AddProvider(new NUnitLoggerProvider());
        });
            
        builder.Services.AddBallwareMetaStorage(storageOptions, connectionString);
        builder.Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
        });

        Application = builder.Build();
    }

    [OneTimeTearDown]
    public async Task TearDownApplication()
    {
        await Application.DisposeAsync();
    }

    [SetUp]
    public async Task SetupTenantId()
    {
        TenantId = Guid.NewGuid();

        using var scope = Application.Services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<IMetadataSeeder>();

        await seeder.SeedCustomerTenantAsync(TenantId, $"Customer_{TenantId.ToString()}");
    }
}