using System.Data;

namespace Infotecs.Mobile.Monitoring.Core.Repositories;

/// <summary>
/// Интерфейс для реализации IUnitOfWork.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Транзакция к базе данных.
    /// </summary>
    public IDbTransaction Transaction { get; }

    /// <summary>
    /// Применить все изменения.
    /// </summary>
    public void Commit();

    /// <summary>
    /// Откатить все изменения.
    /// </summary>
    public void Rollback();
}
