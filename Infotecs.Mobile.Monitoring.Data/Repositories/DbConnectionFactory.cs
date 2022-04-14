using System.Data;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Data.Migrations;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infotecs.Mobile.Monitoring.Data.Repositories;

/// <inheritdoc />
public class DbConnectionFactory : IDbConnectionFactory
{

    private readonly IConfiguration configuration;

    /// <summary>
    /// Создает новый экземпляр класса <see cref="DbConnectionFactory"/> class.
    /// </summary>
    /// <param name="configuration">Конфигурация подключения к базе данных</param>
    public DbConnectionFactory(IConfiguration configuration) => this.configuration = configuration;

    /// <inheritdoc/>
    public IDbConnection CreateConnection()
    {
        var builder = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString(MigrationManager.ConnectionString));

        var conn = new NpgsqlConnection(builder.ConnectionString);

        conn.Open();

        return conn;
    }
}
