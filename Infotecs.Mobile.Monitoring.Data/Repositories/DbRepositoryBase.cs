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
    /// <param name="unitOfWork">Контекст базы данных.</param>
    protected DbRepositoryBase(IUnitOfWork unitOfWork)
    {
        Connection = unitOfWork.Transaction.Connection ??
            throw new ArgumentException(nameof(unitOfWork.Transaction));

        Transaction = unitOfWork.Transaction;
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
