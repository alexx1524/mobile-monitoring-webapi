using System.Data;

namespace Infotecs.Mobile.Monitoring.Core.Repositories;

/// <summary>
/// Интерфейс для реализации IUnitOfWork.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Подключение к базе данных.
    /// </summary>
    public IDbConnection Connection { get; }

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
