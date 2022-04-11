using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infotecs.Mobile.Monitoring.Data.Context;

/// <summary>
/// Контекст для создания подключения к базе данных
/// </summary>
public class DapperContext
{
    /// <summary>
    /// Строка подключения к базе данных для выполнения запросов
    /// </summary>
    public static string ConnectionString = "SqlConnection";

    /// <summary>
    /// Строка подключения к базе данных для создания базы данных
    /// </summary>
    public static string AdminConnectionString = "AdminSqlConnection";

    private readonly IConfiguration configuration;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="configuration">Конфигурация приложения</param>
    public DapperContext(IConfiguration configuration) => this.configuration = configuration;

    /// <summary>
    /// Создание подключения для выполнения запросов данных
    /// </summary>
    /// <returns>Подключение к базе данных</returns>
    public IDbConnection CreateConnection()
        => new NpgsqlConnection(configuration.GetConnectionString(ConnectionString));
}
