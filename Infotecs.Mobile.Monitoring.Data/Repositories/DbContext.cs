using System.Data;
using Infotecs.Mobile.Monitoring.Core.Repositories;

namespace Infotecs.Mobile.Monitoring.Data.Repositories;

/// <summary>
/// Контекст для работы с базой данных.
/// </summary>
public class DbContext : IDbContext
{
    private readonly IDbConnectionFactory dbConnectionFactory;

    private IDbConnection? connection;
    private IDbTransaction? transaction;
    private IUnitOfWork? unitOfWork;

    /// <summary>
    /// Создает новый экземпляр класса <see cref="DbContext"/> class.
    /// </summary>
    /// <param name="dbConnectionFactory">Фабрика создания подключений к БД.</param>
    public DbContext(IDbConnectionFactory dbConnectionFactory) => this.dbConnectionFactory = dbConnectionFactory;

    /// <inheritdoc />
    public IDbConnection Connection =>
        connection ??= OpenConnection();

    /// <inheritdoc />
    public IDbTransaction Transaction =>
        transaction ??= Connection.BeginTransaction();

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork =>
        unitOfWork ??= new UnitOfWork(Transaction);

    /// <inheritdoc />
    public void Commit()
    {
        try
        {
            UnitOfWork.Commit();
        }
        catch
        {
            Rollback();
            throw;
        }
        finally
        {
            Reset();
        }
    }

    /// <inheritdoc />
    public void Rollback()
    {
        try
        {
            UnitOfWork.Rollback();
        }
        finally
        {
            Reset();
        }
    }

    private IDbConnection OpenConnection() => dbConnectionFactory.CreateConnection();

    private void Reset()
    {
        Connection.Close();
        Connection.Dispose();
        Transaction.Dispose();

        connection = null;
        transaction = null;
        unitOfWork = null;
    }
}
