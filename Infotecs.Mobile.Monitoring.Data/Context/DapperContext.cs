using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infotecs.Mobile.Monitoring.Data.Context;

/// <summary>
/// Контекст для создания подключения к базе данных
/// </summary>
public class DapperContext
{
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
        => new NpgsqlConnection(configuration.GetConnectionString("SqlConnection"));
}
