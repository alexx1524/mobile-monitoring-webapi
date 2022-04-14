using System.Data;
using Infotecs.Mobile.Monitoring.Core.Repositories;

namespace Infotecs.Mobile.Monitoring.Data.Repositories;

/// <summary>
/// Контекст для работы с базой данных.
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbConnectionFactory dbConnectionFactory;

    private IDbConnection? connection;
    private IDbTransaction? transaction;

    /// <summary>
    /// Создает новый экземпляр класса <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="dbConnectionFactory">Фабрика создания подключений к БД.</param>
    public UnitOfWork(IDbConnectionFactory dbConnectionFactory) => this.dbConnectionFactory = dbConnectionFactory;

    /// <inheritdoc />
    public IDbConnection Connection =>
        connection ??= OpenConnection();

    /// <inheritdoc />
    public IDbTransaction Transaction =>
        transaction ??= Connection.BeginTransaction();

    /// <inheritdoc />
    public void Commit()
    {
        try
        {
            Transaction.Commit();
        }
        catch (Exception)
        {
            Transaction.Rollback();
            throw;
        }
    }

    /// <inheritdoc />
    public void Rollback() => Transaction.Rollback();

    /// <inheritdoc/>
    public void Dispose()
    {
        Connection.Close();
        Connection.Dispose();
        Transaction.Dispose();

        connection = null;
        transaction = null;
    }

    private IDbConnection OpenConnection() => dbConnectionFactory.CreateConnection();

}
