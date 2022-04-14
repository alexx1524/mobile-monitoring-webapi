using System.Data;
using Infotecs.Mobile.Monitoring.Core.Repositories;

namespace Infotecs.Mobile.Monitoring.Data.Repositories;

/// <summary>
/// Базовый класс репозитория.
/// </summary>
public abstract class DbRepositoryBase
{
    /// <summary>
    /// Создание нового экземпляра класса <see cref="DbRepositoryBase"/> class.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных.</param>
    protected DbRepositoryBase(IDbContext dbContext)
    {
        Connection = dbContext.UnitOfWork.Transaction.Connection ??
            throw new ArgumentException(nameof(dbContext.UnitOfWork.Transaction));

        Transaction = dbContext.UnitOfWork.Transaction;
    }

    /// <summary>
    /// Подключение к базе данных.
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    /// Транзакция.
    /// </summary>
    public IDbTransaction Transaction { get; }

}
