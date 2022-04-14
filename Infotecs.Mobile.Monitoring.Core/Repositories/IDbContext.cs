using System.Data;

namespace Infotecs.Mobile.Monitoring.Core.Repositories;

/// <summary>
/// Контекст для работы с базой данных.
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Подключение к базе данных.
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    /// Транзакция.
    /// </summary>
    public IDbTransaction Transaction { get; }

    /// <summary>
    /// Интерфейс для элемента работы UnitOfWork.
    /// </summary>
    public IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Сохранение изменений.
    /// </summary>
    public void Commit();

    /// <summary>
    /// Откат сделанных изменений.
    /// </summary>
    public void Rollback();
}
