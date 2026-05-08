using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MySql;
using Testcontainers.Redis;

namespace SwarmDemo.Api.IntegrationTests.Fixtures;

public sealed class SwarmDemoApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mysql = new MySqlBuilder("mysql:8.0")
        .WithDatabase("swarm_demo")
        .WithUsername("swarm")
        .WithPassword("swarm")
        .Build();

    private readonly RedisContainer _redis = new RedisBuilder("redis:7-alpine")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _mysql.GetConnectionString(),
                ["ConnectionStrings:Redis"] = $"{_redis.Hostname}:{_redis.GetMappedPublicPort(6379)}"
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _mysql.StartAsync();
        await _redis.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _mysql.DisposeAsync();
        await _redis.DisposeAsync();
        await base.DisposeAsync();
    }
}
