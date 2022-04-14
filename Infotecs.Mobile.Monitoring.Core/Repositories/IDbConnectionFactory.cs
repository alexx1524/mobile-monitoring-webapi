using System.Data;

namespace Infotecs.Mobile.Monitoring.Core.Repositories;

/// <summary>
/// Фабрика создания нового подключения к базе данных.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Создает и открывает новое подключение к базе данных.
    /// </summary>
    /// <returns>Открытое подключение к базе данных.</returns>
    IDbConnection CreateConnection();
}
