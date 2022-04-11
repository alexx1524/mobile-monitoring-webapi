using System.Reflection;
using FluentMigrator.Runner;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Core.Services;
using Infotecs.Mobile.Monitoring.Data.Context;
using Infotecs.Mobile.Monitoring.Data.Migrations;
using Infotecs.Mobile.Monitoring.Data.Repositories;
using Infotecs.Mobile.Monitoring.Data.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infotecs.Mobile.Monitoring.Data;

public static class ServiceCollectionExtension
{
    public static IServiceCollection InitializeDataServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddPostgres()
                .WithGlobalConnectionString(configuration.GetConnectionString("SqlConnection"))
                .ScanIn(Assembly.GetAssembly(typeof(MigrationManager))).For.Migrations());

        serviceCollection.AddSingleton<DapperContext>();

        serviceCollection.AddScoped<IMonitoringDataRepository, MonitoringDataRepository>();

        serviceCollection.AddTransient<IMonitoringService, MonitoringService>();

        return serviceCollection;
    }
}
