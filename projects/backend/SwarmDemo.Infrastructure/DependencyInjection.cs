using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Infrastructure.Caching;
using SwarmDemo.Infrastructure.Persistence;
using SwarmDemo.Infrastructure.Persistence.Repositories;

namespace SwarmDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnection = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                dbConnection,
                new MySqlServerVersion(new Version(8, 0, 36)),
                mysql => mysql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IProductsRepository, ProductsRepository>();

        var redisConnection = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is missing.");

        var redisConfig = new ConfigurationOptions
        {
            EndPoints = { redisConnection },
            AbortOnConnectFail = false,
            ConnectTimeout = 5000,
            SyncTimeout = 3000,
            AsyncTimeout = 3000,
            ConnectRetry = 3,
            KeepAlive = 60
        };

        IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConfig);
        services.AddSingleton(multiplexer);

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConnectionMultiplexerFactory = () => Task.FromResult(multiplexer);
            options.InstanceName = "swarm-demo:";
        });

        services.AddScoped<IProductsCache, RedisProductsCache>();

        return services;
    }
}
