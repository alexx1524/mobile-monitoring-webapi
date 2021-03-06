using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infotecs.Mobile.Monitoring.Data.Migrations;

/// <summary>
/// Класс дял создания базы данных и выполнения миграций.
/// </summary>
public static class MigrationManager
{
    /// <summary>
    /// Строка подключения к базе данных для выполнения запросов.
    /// </summary>
    public static string ConnectionString = "SqlConnection";

    /// <summary>
    /// Строка подключения к базе данных для создания.
    /// </summary>
    public static string AdminConnectionString = "AdminSqlConnection";

    /// <summary>
    /// Метод выполнения миграции базы данных.
    /// </summary>
    /// <param name="host">Хост для выполнения миграции.</param>
    /// <typeparam name="T">Тип, для которого запрашивается интерфейс логгирования.</typeparam>
    /// <returns>Хост.</returns>
    public static IHost MigrateDatabase<T>(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();

        IServiceProvider services = scope.ServiceProvider;

        var migrationService = services.GetRequiredService<IMigrationRunner>();
        var logger = services.GetRequiredService<ILogger<T>>();
        var configuration = services.GetService<IConfiguration>();

        try
        {
            string? connectionString = configuration.GetConnectionString(ConnectionString);
            string? adminConnectionString = configuration.GetConnectionString(AdminConnectionString);

            string databaseName = ExtractDatabaseName(connectionString);

            if (!DatabaseExists(adminConnectionString, databaseName, logger))
            {
                logger.LogInformation($"Создание базы данных");

                CreateDatabase(adminConnectionString, databaseName, logger);
            }
            else
            {
                logger.LogInformation("База данных уже создана");
            }

            migrationService.ListMigrations();
            migrationService.MigrateUp();

            logger.LogInformation("Миграция базы данных выполнена");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка миграции базы данных");
            throw;
        }

        return host;
    }

    /// <summary>
    /// Получение имени базы данных из строки подключения.
    /// </summary>
    /// <param name="connectionString">Строка подключения к базе данных.</param>
    /// <returns>Наименование базы данных.</returns>
    private static string ExtractDatabaseName(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder()
        {
            ConnectionString = connectionString,
        };

        return builder.Database!;
    }

    /// <summary>
    /// Метод для проверки, что база данных уже создана.
    /// </summary>
    /// <param name="connectionString">Строка подключения к базе данных.</param>
    /// <param name="databaseName">Наименование базы данных.</param>
    /// <param name="logger">Интерфейс для логгирования.</param>
    /// <returns>Создана база данных или нет.</returns>
    private static bool DatabaseExists(string connectionString, string databaseName, ILogger logger)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var query = $"SELECT DATNAME FROM pg_catalog.pg_database WHERE DATNAME = '{databaseName}'";

            using (var command = new NpgsqlCommand(query, connection))
            {
                try
                {
                    connection.Open();

                    object? i = command.ExecuteScalar();

                    return i != null && i.ToString()!.Equals(databaseName);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Ошибка проверки базы данных");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// Метод создания базы данных.
    /// </summary>
    /// <param name="connectionString">Строка подключения к базе данных.</param>
    /// <param name="databaseName">Наименование базы данных.</param>
    /// <param name="logger">Интерфейс для логгирования.</param>
    private static void CreateDatabase(string connectionString, string databaseName, ILogger logger)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var query = $"CREATE DATABASE \"{databaseName}\" WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

            using (var command = new NpgsqlCommand(query, connection))
            {
                try
                {
                    connection.Open();

                    command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Ошибка создания базы данных");

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }

}
