using System.Data;
using Infotecs.Mobile.Monitoring.Core.Repositories;

namespace Infotecs.Mobile.Monitoring.Data.Repositories;

/// <summary>
/// Класс для выполнения транзаций в базу данных.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Создает экземпляр класса <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="transaction">Транзакция к базе данных.</param>
    public UnitOfWork(IDbTransaction transaction) => Transaction = transaction;

    /// <inheritdoc />
    public IDbTransaction Transaction { get; }

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

}
