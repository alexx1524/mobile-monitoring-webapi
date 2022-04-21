using System.Reflection;
using FluentMigrator.Runner;
using Infotecs.Mobile.Monitoring.Core.ClientsInforming;
using Infotecs.Mobile.Monitoring.Core.Messages;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Core.Services;
using Infotecs.Mobile.Monitoring.Data.Hubs;
using Infotecs.Mobile.Monitoring.Data.Messages;
using Infotecs.Mobile.Monitoring.Data.Migrations;
using Infotecs.Mobile.Monitoring.Data.Repositories;
using Infotecs.Mobile.Monitoring.Data.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;

namespace Infotecs.Mobile.Monitoring.Data;

/// <summary>
/// Расширение коллекции сервисов.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Добавление сервисов данных в DI.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Коллекция сервисов.</returns>
    public static IServiceCollection AddDataServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddPostgres()
                .WithGlobalConnectionString(configuration.GetConnectionString(MigrationManager.ConnectionString))
                .ScanIn(Assembly.GetAssembly(typeof(MigrationManager))).For.Migrations());

        serviceCollection.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<IMonitoringDataRepository, MonitoringDataRepository>();

        serviceCollection.AddTransient<IMonitoringService, MonitoringService>();
        serviceCollection.AddTransient<IChangeNotifier, ChangeNotifier>();
        serviceCollection.AddTransient<IMessagePublisher, RabbitMessagePublisher>();

        IConfigurationSection? rabbitMqSection = configuration.GetSection("RabbitMq");
        IConfigurationSection? exchangeSection = configuration.GetSection("RabbitMqNodeEventsExchange");

        serviceCollection.AddRabbitMqProducer(rabbitMqSection)
            .AddProductionExchange("NodeEventsExchange", exchangeSection);

        return serviceCollection;
    }
}
