using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Specwright.Core.Workflows.Contracts;
using Specwright.Infrastructure.Capabilities;
using Specwright.Infrastructure.Persistence;

namespace Specwright.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public const string DatabaseConnectionName = "specwrightdb";

    public static TBuilder AddSpecwrightInfrastructure<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<SpecwrightDbContext>(DatabaseConnectionName);
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddScoped<IRecordExecutionCapability, RecordExecutionCapability>();

        return builder;
    }

    public static async Task InitializeSpecwrightInfrastructureAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SpecwrightDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }
}
