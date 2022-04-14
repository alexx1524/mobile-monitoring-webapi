using System.Data;
using Infotecs.Mobile.Monitoring.Core.Repositories;

namespace Infotecs.Mobile.Monitoring.Data.Repositories;

/// <inheritdoc />
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly Func<IDbConnection> connectionFactory;

    /// <summary>
    /// Создает новый экземпляр класса <see cref="DbConnectionFactory"/> class.
    /// </summary>
    /// <param name="connectionFactory">Функция создания нового подключения.</param>
    public DbConnectionFactory(Func<IDbConnection> connectionFactory) =>
        this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    /// <inheritdoc/>
    public IDbConnection CreateOpenConnection() => connectionFactory();
}
